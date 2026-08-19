using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using DiscordRelay.Core.Interfaces;
using DiscordRelay.Core.Models;

namespace DiscordRelay.Infrastructure.Queue
{
    public class FileQueueWatcher : IQueueWatcher
    {
        private readonly RelaySettings _settings;
        private string? _currentBatchFile;

        public FileQueueWatcher(RelaySettings settings)
        {
            _settings = settings;
        }

        public async Task<IEnumerable<string>> GetNewPayloadsAsync()
        {
            if (string.IsNullOrEmpty(_settings.QueueDirectory) || !Directory.Exists(_settings.QueueDirectory))
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error: Queue directory not found: {_settings.QueueDirectory}");
                return Enumerable.Empty<string>();
            }

            // Look for batch files created by the Mod
            var files = Directory.GetFiles(_settings.QueueDirectory, "Discord_Batch_*.json")
                                 .OrderBy(f => f) // Process in chronological order
                                 .ToList();

            if (files.Count == 0) return Enumerable.Empty<string>();

            _currentBatchFile = files[0];
            var payloads = new List<string>();

            try
            {
                // Read the oldest batch
                payloads.AddRange(await File.ReadAllLinesAsync(_currentBatchFile));
                
                // We DON'T delete yet. We wait for the Orchestrator to confirm success.
                return payloads;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error reading batch {_currentBatchFile}: {ex.Message}");
                return Enumerable.Empty<string>();
            }
        }

        public async Task ConfirmSuccessAsync()
        {
            if (string.IsNullOrEmpty(_currentBatchFile)) return;

            try
            {
                if (File.Exists(_currentBatchFile))
                {
                    File.Delete(_currentBatchFile);
                }
                _currentBatchFile = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Error deleting batch {_currentBatchFile}: {ex.Message}");
            }
            await Task.CompletedTask;
        }

        public async Task RequeuePayloadsAsync(IEnumerable<string> payloads)
        {
            // With Multi-File, we don't need to write back to the file. 
            // We just don't call ConfirmSuccessAsync(), and the file remains for the next attempt.
            _currentBatchFile = null;
            await Task.CompletedTask;
        }
    }
}
