using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DiscordRelay.Core.Interfaces;
using DiscordRelay.Core.Models;
using Newtonsoft.Json.Linq;

namespace DiscordRelay.Infrastructure.Discord
{
    public class DiscordHttpClient : IDiscordClient
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly RelaySettings _settings;

        public DiscordHttpClient(RelaySettings settings)
        {
            _settings = settings;
        }

        public async Task<DiscordResponse> SendPayloadAsync(string json)
        {
            var result = new DiscordResponse();
            
            try
            {
                var payload = JObject.Parse(json);
                string channelType = payload["channel_type"]?.ToString() ?? "Default";
                
                payload.Remove("channel_type");
                string finalJson = payload.ToString();

                string? targetUrl = _settings.DefaultDiscordUrl;

                if (_settings.Channels.TryGetValue(channelType, out var config))
                {
                    if (!string.IsNullOrEmpty(config.DiscordUrl)) targetUrl = config.DiscordUrl;
                }

                if (string.IsNullOrEmpty(targetUrl))
                {
                    result.ErrorMessage = $"No target URL found for channel type: {channelType}";
                    return result;
                }

                var content = new StringContent(finalJson, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync(targetUrl, content);
                
                result.StatusCode = (int)response.StatusCode;
                result.Success = response.IsSuccessStatusCode;
                result.Headers = response.Headers;

                if (!response.IsSuccessStatusCode)
                {
                    result.ErrorMessage = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Discord API Error ({response.StatusCode}) for {channelType}: {result.ErrorMessage}");
                }

                return result;
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Processing/Network Error: {ex.Message}");
                return result;
            }
        }
    }
}
