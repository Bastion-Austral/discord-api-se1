using System;
using System.Collections.Generic;
using Sandbox.ModAPI;
using VRage.Utils;
using System.IO;

namespace DiscordAPI.Core
{
    public class FileQueueProvider
    {
        private const string BatchPrefix = "Discord_Batch_";
        private readonly int _maxQueueSize;
        private readonly List<string> _pendingPayloads = new List<string>();
        private readonly object _lock = new object();

        public FileQueueProvider(int maxQueueSize)
        {
            _maxQueueSize = maxQueueSize;
        }

        public void Enqueue(string json)
        {
            lock (_lock)
            {
                // Cache the events in memory
                if (_pendingPayloads.Count >= _maxQueueSize) return;
                _pendingPayloads.Add(json);
            }
        }

        public void ProcessQueue()
        {
            List<string> toProcess;
            lock (_lock)
            {
                if (_pendingPayloads.Count == 0) return;
                toProcess = new List<string>(_pendingPayloads);
                _pendingPayloads.Clear();
            }

            try
            {
                // Create a UNIQUE file for this batch. O(1) operation (Write-Only).
                // Use a combination of DateTime ticks and a small random for uniqueness
                string fileName = $"{BatchPrefix}{DateTime.UtcNow.Ticks}.json";

                using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage(fileName, typeof(FileQueueProvider)))
                {
                    foreach (var payload in toProcess)
                    {
                        writer.WriteLine(payload);
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole($"DiscordAPI Error: Failed to write batch file: {ex.Message}");
            }
        }
    }
}
