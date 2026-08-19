using System;
using System.IO;
using Newtonsoft.Json;
using DiscordRelay.Core.Interfaces;
using DiscordRelay.Core.Models;

namespace DiscordRelay.Infrastructure.Settings
{
    public class SettingsProvider : ISettingsProvider
    {
        private const string ConfigPath = "relay_config.json";

        public RelaySettings GetSettings(string[] args)
        {
            var settings = new RelaySettings();

            // 1. Environment
            settings.QueueDirectory = Environment.GetEnvironmentVariable("DISCORD_QUEUE_PATH");
            settings.DefaultDiscordUrl = Environment.GetEnvironmentVariable("DISCORD_WEBHOOK_URL");

            // 2. Config File
            if (File.Exists(ConfigPath))
            {
                try
                {
                    var config = JsonConvert.DeserializeObject<RelaySettings>(File.ReadAllText(ConfigPath));
                    if (config != null)
                    {
                        if (!string.IsNullOrEmpty(config.QueueDirectory)) settings.QueueDirectory = config.QueueDirectory;
                        if (!string.IsNullOrEmpty(config.DefaultDiscordUrl)) settings.DefaultDiscordUrl = config.DefaultDiscordUrl;
                        if (config.PollingIntervalMs > 0) settings.PollingIntervalMs = config.PollingIntervalMs;
                        if (config.Channels != null) settings.Channels = config.Channels;
                    }
                }
                catch (Exception ex) { Console.WriteLine("Warning: Failed to load relay_config.json: " + ex.Message); }
            }

            // 3. Command Line
            if (args.Length >= 1) settings.QueueDirectory = args[0];
            if (args.Length >= 2) settings.DefaultDiscordUrl = args[1];

            return settings;
        }
    }
}
