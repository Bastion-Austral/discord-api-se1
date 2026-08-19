using System;
using System.Collections.Generic;
using VRage.Game.Components;
using Sandbox.ModAPI;
using VRage.Utils;
using DiscordAPI.API;
using DiscordAPI.Storage;
using Sandbox.Game;

namespace DiscordAPI.Core
{
    [MySessionComponentDescriptor(MyUpdateOrder.AfterSimulation)]
    public class DiscordService : MySessionComponentBase, IDiscordAPI
    {
        public static DiscordService Instance;

        private ConfigManager _configManager;
        private FileQueueProvider _queueProvider;
        private GameEventsManager _eventsManager;
        private ChatCommandHandler _chatHandler;
        private bool _isInitialized = false;

        public static readonly long MOD_API_ID = GetDeterministicLong("DiscordAPI_MOD_API_ID");
        public static string MOD_VERSION = "Unknown";

        private static long GetDeterministicLong(string str)
        {
            ulong hash = 5381;
            foreach (char c in str)
            {
                hash = ((hash << 5) + hash) + c;
            }
            return (long)hash;
        }

        private void LoadModVersion()
        {
            try
            {
                ulong contextId = 0;
                ulong.TryParse(ModContext.ModId, out contextId);

                VRage.Game.MyObjectBuilder_Checkpoint.ModItem modItem = default(VRage.Game.MyObjectBuilder_Checkpoint.ModItem);
                bool found = false;

                if (MyAPIGateway.Session?.Mods != null)
                {
                    foreach (var item in MyAPIGateway.Session.Mods)
                    {
                        if ((item.PublishedFileId != 0 && item.PublishedFileId == contextId) || (item.Name == ModContext.ModId))
                        {
                            modItem = item;
                            found = true;
                            break;
                        }
                    }
                }

                if (found)
                {
                    var utils = MyAPIGateway.Utilities;
                    if (utils.FileExistsInModLocation("metadata.mod", modItem))
                    {
                        using (var reader = utils.ReadFileInModLocation("metadata.mod", modItem))
                        {
                            var content = reader.ReadToEnd();
                            int startTag = content.IndexOf("<ModVersion>");
                            int endTag = content.IndexOf("</ModVersion>");
                            
                            if (startTag != -1 && endTag != -1)
                            {
                                int start = startTag + "<ModVersion>".Length;
                                MOD_VERSION = content.Substring(start, endTag - start).Trim();
                                MyLog.Default.WriteLine($"DiscordAPI: Version loaded successfully from metadata.mod: {MOD_VERSION}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLine($"DiscordAPI: Error loading metadata.mod: {ex.Message}");
            }
        }

        public override void LoadData()
        {
            if (!MyAPIGateway.Session.IsServer) return;
            Instance = this;

            LoadModVersion();

            _configManager = new ConfigManager();
            _configManager.Load();

            if (!_configManager.Config.Enabled)
            {
                MyLog.Default.WriteLineAndConsole("DiscordAPI: Mod is disabled in config.");
                return;
            }

            _queueProvider = new FileQueueProvider(_configManager.Config.MaxQueueSize);
            _eventsManager = new GameEventsManager(this);
            _chatHandler = new ChatCommandHandler();
            
            _isInitialized = true;
            MyLog.Default.WriteLineAndConsole("DiscordAPI: Service initialized on server. Relay mode active.");

            MyAPIGateway.Utilities.RegisterMessageHandler(MOD_API_ID, OnModMessageReceived);
        }

        public override void UpdateAfterSimulation()
        {
            if (!_isInitialized) return;
            
            // Only process queue based on configured frequency
            if (MyAPIGateway.Session.GameplayFrameCounter % _configManager.Config.UpdateFrequencyTicks == 0)
            {
                _queueProvider.ProcessQueue();
            }
        }

        protected override void UnloadData()
        {
            MyAPIGateway.Utilities.UnregisterMessageHandler(MOD_API_ID, OnModMessageReceived);
            _eventsManager?.Unload();
            _chatHandler?.Unload();
            Instance = null;
            _isInitialized = false;
        }

        private void OnModMessageReceived(object message)
        {
            try
            {
                var payload = message as object[];
                if (payload == null || payload.Length == 0) return;

                string command = payload[0] as string;
                if (string.IsNullOrEmpty(command)) return;

                if (command == "SendMessage")
                {
                    if (payload.Length >= 2)
                    {
                        string content = payload[1] as string;
                        string channelType = payload.Length > 2 ? payload[2] as string : "Default";
                        SendMessage(content, null, channelType);
                    }
                }
                else if (command == "SendEmbed")
                {
                    if (payload.Length >= 2)
                    {
                        var dict = payload[1] as Dictionary<string, object>;
                        string channelType = payload.Length > 2 ? payload[2] as string : "Default";
                        var embed = ParseEmbedFromDict(dict);
                        if (embed != null)
                        {
                            SendEmbed(embed, channelType);
                        }
                    }
                }
                else if (command == "GetOnlinePlayers")
                {
                    if (payload.Length >= 2)
                    {
                        var callback = payload[1] as Action<List<string>>;
                        if (callback != null)
                        {
                            var players = new List<IMyPlayer>();
                            MyAPIGateway.Players.GetPlayers(players);
                            var names = new List<string>();
                            foreach (var p in players)
                            {
                                if (p != null) names.Add(p.DisplayName);
                            }
                            callback(names);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MyLog.Default.WriteLineAndConsole($"DiscordAPI: Error handling ModMessage: {ex.Message}");
            }
        }

        private DiscordEmbed ParseEmbedFromDict(Dictionary<string, object> dict)
        {
            if (dict == null) return null;
            var embed = new DiscordEmbed();
            
            object val;
            if (dict.TryGetValue("Title", out val) && val is string) embed.Title = val as string;
            if (dict.TryGetValue("Description", out val) && val is string) embed.Description = val as string;
            if (dict.TryGetValue("Url", out val) && val is string) embed.Url = val as string;
            if (dict.TryGetValue("Color", out val) && val is int) embed.Color = (int)val;
            
            if (dict.TryGetValue("FooterText", out val) && val is string)
            {
                embed.Footer = new DiscordFooter { Text = val as string };
                object iconVal;
                if (dict.TryGetValue("FooterIcon", out iconVal) && iconVal is string)
                    embed.Footer.IconUrl = iconVal as string;
            }
            
            if (dict.TryGetValue("ThumbnailUrl", out val) && val is string)
            {
                embed.Thumbnail = new DiscordThumbnail { Url = val as string };
            }
            
            if (dict.TryGetValue("ImageUrl", out val) && val is string)
            {
                embed.Image = new DiscordImage { Url = val as string };
            }
            
            if (dict.TryGetValue("AuthorName", out val) && val is string)
            {
                embed.Author = new DiscordAuthor { Name = val as string };
                object authorUrlVal, authorIconVal;
                if (dict.TryGetValue("AuthorUrl", out authorUrlVal) && authorUrlVal is string)
                    embed.Author.Url = authorUrlVal as string;
                if (dict.TryGetValue("AuthorIcon", out authorIconVal) && authorIconVal is string)
                    embed.Author.IconUrl = authorIconVal as string;
            }
            
            if (dict.TryGetValue("Fields", out val))
            {
                var fieldsList = val as System.Collections.IEnumerable;
                if (fieldsList != null)
                {
                    foreach (var rawField in fieldsList)
                    {
                        var fieldArr = rawField as object[];
                        if (fieldArr != null && fieldArr.Length >= 2)
                        {
                            string name = fieldArr[0] as string;
                            string fieldValue = fieldArr[1] as string;
                            bool inline = fieldArr.Length > 2 ? (bool)fieldArr[2] : false;
                            embed.AddField(name, fieldValue, inline);
                        }
                    }
                }
            }
            
            return embed;
        }

        public void SendMessage(string content, DiscordEmbed embed = null, string channelType = "Default")
        {
            if (!_isInitialized) return;
            string json = DiscordPayloads.Serialize(content, embed, channelType);
            _queueProvider.Enqueue(json);
            MyLog.Default.WriteLineAndConsole($"DiscordAPI: Queued payload for channel '{channelType}'.");
        }

        public void SendEmbed(DiscordEmbed embed, string channelType = "Default")
        {
            SendMessage(null, embed, channelType);
        }

        public DiscordEmbed CreateEmbed()
        {
            return new DiscordEmbed();
        }
    }
}
