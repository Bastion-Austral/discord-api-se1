using System;
using System.Xml.Serialization;

namespace DiscordAPI.Storage
{
    [XmlRoot("DiscordConfig")]
    public class DiscordConfig
    {
        public bool Enabled = true;
        public int MaxQueueSize = 1000;
        public int UpdateFrequencyTicks = 60;
        
        [XmlElement("Logging")]
        public LoggingConfig Logging = new LoggingConfig();
    }

    public class LoggingConfig
    {
        public bool Debug = false;
        public bool Trace = false;
    }
}
