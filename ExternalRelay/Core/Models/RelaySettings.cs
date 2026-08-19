using System.Collections.Generic;

namespace DiscordRelay.Core.Models
{
    public class RelaySettings
    {
        public string? QueueDirectory { get; set; }
        public string? DefaultDiscordUrl { get; set; }
        public int PollingIntervalMs { get; set; } = 2000;
        public int RateLimitDelayMs { get; set; } = 1000;

        public Dictionary<string, ChannelConfig> Channels { get; set; } = new Dictionary<string, ChannelConfig>();
    }

    public class ChannelConfig
    {
        public string? DiscordUrl { get; set; }
    }
}
