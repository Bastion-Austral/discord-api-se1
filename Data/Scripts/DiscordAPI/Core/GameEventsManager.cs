using System;
using Sandbox.ModAPI;
using VRage.Game.ModAPI;
using DiscordAPI.API;
using VRage.Game;
using VRage.Utils;
using Sandbox.Game;
using System.Collections.Generic;

namespace DiscordAPI.Core
{
    public class GameEventsManager
    {
        private readonly DiscordService _discord;

        public GameEventsManager(DiscordService discord)
        {
            _discord = discord;
            
            MyAPIGateway.Utilities.MessageEntered += OnMessageEntered;
            
            MyVisualScriptLogicProvider.PlayerConnected += OnPlayerConnected;
            MyVisualScriptLogicProvider.PlayerDisconnected += OnPlayerDisconnected;
            MyVisualScriptLogicProvider.PlayerDied += OnPlayerDied;
        }

        public void Unload()
        {
            MyAPIGateway.Utilities.MessageEntered -= OnMessageEntered;
            MyVisualScriptLogicProvider.PlayerConnected -= OnPlayerConnected;
            MyVisualScriptLogicProvider.PlayerDisconnected -= OnPlayerDisconnected;
            MyVisualScriptLogicProvider.PlayerDied -= OnPlayerDied;
        }

        private void OnMessageEntered(string messageText, ref bool sendToDefault)
        {
            if (messageText.StartsWith("/")) return; 

            string name = MyAPIGateway.Session.Player?.DisplayName ?? "Unknown";
            _discord.SendMessage($"**{name}**: {messageText}", null, "Chat");
        }

        private void OnPlayerConnected(long identityId)
        {
            string name = GetPlayerName(identityId);
            
            var embed = new DiscordEmbed
            {
                Title = "Player Connected",
                Description = $"{name} has joined the server.",
                Color = 0x00FF00 
            };
            _discord.SendEmbed(embed, "Connections");
        }

        private void OnPlayerDisconnected(long identityId)
        {
            string name = GetPlayerName(identityId);

            var embed = new DiscordEmbed
            {
                Title = "Player Disconnected",
                Description = $"{name} has left the server.",
                Color = 0xFF0000 
            };
            _discord.SendEmbed(embed, "Connections");
        }

        private void OnPlayerDied(long identityId)
        {
            string name = GetPlayerName(identityId);

            var embed = new DiscordEmbed
            {
                Title = "Player Died",
                Description = $"{name} has died.",
                Color = 0x555555 
            };
            _discord.SendEmbed(embed, "Deaths");
        }

        private string GetPlayerName(long identityId)
        {
            var identities = new List<IMyIdentity>();
            MyAPIGateway.Players.GetAllIdentites(identities, id => id.IdentityId == identityId);
            if (identities.Count > 0 && identities[0] != null)
            {
                return identities[0].DisplayName;
            }

            var player = GetPlayerByIdentity(identityId);
            if (player != null)
            {
                return player.DisplayName;
            }

            return "Unknown";
        }

        private IMyPlayer GetPlayerByIdentity(long identityId)
        {
            var players = new List<IMyPlayer>();
            MyAPIGateway.Players.GetPlayers(players, p => p.IdentityId == identityId);
            return players.Count > 0 ? players[0] : null;
        }
    }
}
