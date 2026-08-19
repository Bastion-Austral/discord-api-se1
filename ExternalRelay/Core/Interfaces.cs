using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiscordRelay.Core.Models;

namespace DiscordRelay.Core.Interfaces
{
    public interface ISettingsProvider
    {
        RelaySettings GetSettings(string[] args);
    }

    public interface IQueueWatcher
    {
        Task<IEnumerable<string>> GetNewPayloadsAsync();
        Task RequeuePayloadsAsync(IEnumerable<string> payloads);
        Task ConfirmSuccessAsync();
    }

    public interface IDiscordClient
    {
        Task<DiscordResponse> SendPayloadAsync(string json);
    }

    public interface IRateLimiter
    {
        Task WaitIfNeededAsync();
        void UpdateLimits(HttpResponseHeaders headers);
    }

    public class DiscordResponse
    {
        public bool Success { get; set; }
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public HttpResponseHeaders? Headers { get; set; }
    }
}
