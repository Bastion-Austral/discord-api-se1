using System;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using DiscordAPI.API;

namespace DiscordAPI.Core
{
    public class ChatCommandHandler
    {
        private const string CommandPrefix = "/discord";

        public ChatCommandHandler()
        {
            MyAPIGateway.Utilities.MessageEntered += OnMessageEntered;
        }

        public void Unload()
        {
            MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
        }

        private void OnMessageEntered(string messageText, ref bool sendToDefault)
        {
            if (messageText.StartsWith(CommandPrefix))
            {
                sendToDefault = false; // Don't show in game chat

                if (DiscordService.Instance == null)
                {
                    MyAPIGateway.Utilities.ShowMessage("DiscordAPI", "Service not initialized.");
                    return;
                }

                string content = messageText.Substring(CommandPrefix.Length).Trim();
                if (string.IsNullOrEmpty(content))
                {
                    MyAPIGateway.Utilities.ShowMessage("DiscordAPI", "Usage: /discord <message>");
                    return;
                }

                // Send to Discord
                var embed = new DiscordEmbed
                {
                    Title = "In-Game Command",
                    Description = content,
                    Color = 0x00A2FF, // Blue
                    Author = new DiscordAuthor 
                    { 
                        Name = MyAPIGateway.Session.Player?.DisplayName ?? "Server Admin" 
                    }
                };

                DiscordService.Instance.SendEmbed(embed);
                MyAPIGateway.Utilities.ShowMessage("DiscordAPI", "Message queued for Relay.");
            }
        }
    }
}
