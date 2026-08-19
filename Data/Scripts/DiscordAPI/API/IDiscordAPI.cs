using System;
using DiscordAPI.API;

namespace DiscordAPI.API
{
    public interface IDiscordAPI
    {
        void SendMessage(string content, DiscordEmbed embed = null, string channelType = "Default");
        void SendEmbed(DiscordEmbed embed, string channelType = "Default");
        
        /// <summary>
        /// Create a new embed builder
        /// </summary>
        DiscordEmbed CreateEmbed();
    }
}
