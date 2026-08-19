using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DiscordRelay.Core.Interfaces;
using DiscordRelay.Core.Models;

namespace DiscordRelay.Core
{
    public class RelayOrchestrator
    {
        private readonly RelaySettings _settings;
        private readonly IQueueWatcher _watcher;
        private readonly IDiscordClient _discord;
        private readonly IRateLimiter _limiter;

        public RelayOrchestrator(RelaySettings settings, IQueueWatcher watcher, IDiscordClient discord, IRateLimiter limiter)
        {
            _settings = settings;
            _watcher = watcher;
            _discord = discord;
            _limiter = limiter;
        }

        public async Task StartAsync()
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Relay Orchestrator started. Multi-File Resilience mode.");

            while (true)
            {
                var payloads = (await _watcher.GetNewPayloadsAsync()).ToList();
                
                if (payloads.Count == 0)
                {
                    await Task.Delay(_settings.PollingIntervalMs);
                    continue;
                }

                bool batchSuccess = true;

                for (int i = 0; i < payloads.Count; i++)
                {
                    var payload = payloads[i];
                    
                    await _limiter.WaitIfNeededAsync();

                    var response = await _discord.SendPayloadAsync(payload);
                    
                    if (response.Headers != null)
                    {
                        _limiter.UpdateLimits(response.Headers);
                    }

                    if (response.Success)
                    {
                        await Task.Delay(200); 
                    }
                    else
                    {
                        if (response.StatusCode == 429)
                        {
                            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] 429 detected. Retrying message...");
                            i--; 
                            continue;
                        }

                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Discord/Network error. Aborting batch.");
                        batchSuccess = false;
                        break;
                    }
                }

                if (batchSuccess)
                {
                    await _watcher.ConfirmSuccessAsync();
                }
                else
                {
                    // If batch failed, we don't confirm. The watcher keeps the file for next time.
                    await Task.Delay(5000); 
                }

                // Process next batch immediately if there were more
                await Task.Delay(500); 
            }
        }
    }
}
