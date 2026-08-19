using System;
using System.Threading.Tasks;
using DiscordRelay.Core;
using DiscordRelay.Infrastructure.Settings;
using DiscordRelay.Infrastructure.Queue;
using DiscordRelay.Infrastructure.Discord;

namespace DiscordRelay
{
    class Program
    {
        public const string RELAY_VERSION = "1.0.0";

        static async Task Main(string[] args)
        {
            try
            {
                Console.WriteLine($"=== Space Engineers Discord Relay v{RELAY_VERSION} (SOLID + Rate Limiting) ===");

                var settingsProvider = new SettingsProvider();
                var settings = settingsProvider.GetSettings(args);

                if (string.IsNullOrEmpty(settings.QueueDirectory) || (string.IsNullOrEmpty(settings.DefaultDiscordUrl) && settings.Channels.Count == 0))
                {
                    Console.WriteLine("Error: Missing required settings.");
                    if (string.IsNullOrEmpty(settings.QueueDirectory)) Console.WriteLine(" - QueueDirectory is empty.");
                    if (string.IsNullOrEmpty(settings.DefaultDiscordUrl) && settings.Channels.Count == 0) Console.WriteLine(" - No Discord URLs configured (neither Default nor Channels).");
                    Console.WriteLine("\nCheck your relay_config.json or environment variables.");
                    return;
                }

                var watcher = new FileQueueWatcher(settings);
                var discordClient = new DiscordHttpClient(settings);
                var rateLimiter = new DiscordRateLimiter(); // New Rate Limiter implementation

                var orchestrator = new RelayOrchestrator(settings, watcher, discordClient, rateLimiter);
                
                await orchestrator.StartAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FATAL ERROR: {ex.Message}");
            }
            finally
            {
                Console.WriteLine("\nPresiona Enter para salir...");
                Console.ReadLine();
            }
        }
    }
}
