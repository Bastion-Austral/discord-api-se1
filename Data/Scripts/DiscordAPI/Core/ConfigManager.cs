using System;
using System.IO;
using VRage.Game.ModAPI;
using VRage.Utils;
using DiscordAPI.Storage;

namespace DiscordAPI.Core
{
    public class ConfigManager
    {
        private const string ConfigFileName = "DiscordAPI_Config.xml";
        public DiscordConfig Config { get; private set; }

        public void Load()
        {
            try
            {
                if (MyAPIGateway.Utilities.FileExistsInWorldStorage(ConfigFileName, typeof(DiscordConfig)))
                {
                    using (var reader = MyAPIGateway.Utilities.ReadFileInWorldStorage(ConfigFileName, typeof(DiscordConfig)))
                    {
                        string xmlText = reader.ReadToEnd();
                        Config = MyAPIGateway.Utilities.SerializeFromXML<DiscordConfig>(xmlText);
                    }
                }

                if (Config == null)
                {
                    Config = new DiscordConfig();
                    Save();
                    MyLog.Default.WriteLineAndConsole("DiscordAPI: Created default configuration file.");
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole($"DiscordAPI: Error loading configuration: {ex.Message}");
                Config = new DiscordConfig(); // Fallback to default if error, but we should probably stop if security is at risk
                throw new InvalidOperationException("Failed to load Discord configuration. Check logs.", ex);
            }
        }

        public void Save()
        {
            try
            {
                using (var writer = MyAPIGateway.Utilities.WriteFileInWorldStorage(ConfigFileName, typeof(DiscordConfig)))
                {
                    string xmlText = MyAPIGateway.Utilities.SerializeToXML(Config);
                    writer.Write(xmlText);
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole($"DiscordAPI: Error saving configuration: {ex.Message}");
            }
        }
    }
}
