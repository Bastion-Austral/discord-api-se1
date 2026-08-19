using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiscordRelay.Core.Interfaces;

namespace DiscordRelay.Infrastructure.Discord
{
    public class DiscordRateLimiter : IRateLimiter
    {
        private int _remaining = 1;
        private DateTime _resetTime = DateTime.MinValue;
        private readonly object _lock = new object();

        public async Task WaitIfNeededAsync()
        {
            DateTime now = DateTime.UtcNow;
            int waitMs = 0;

            lock (_lock)
            {
                if (_remaining <= 0 && now < _resetTime)
                {
                    waitMs = (int)(_resetTime - now).TotalMilliseconds + 100; // Buffer
                }
            }

            if (waitMs > 0)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Rate limit reached. Waiting {waitMs}ms...");
                await Task.Delay(waitMs);
            }
        }

        public void UpdateLimits(HttpResponseHeaders headers)
        {
            lock (_lock)
            {
                if (headers.TryGetValues("X-RateLimit-Remaining", out var remainingValues))
                {
                    int.TryParse(remainingValues.FirstOrDefault(), out _remaining);
                }

                if (headers.TryGetValues("X-RateLimit-Reset-After", out var resetAfterValues))
                {
                    if (double.TryParse(resetAfterValues.FirstOrDefault(), out double resetAfter))
                    {
                        _resetTime = DateTime.UtcNow.AddSeconds(resetAfter);
                    }
                }
                
                // 429 Retry-After handling
                if (headers.TryGetValues("Retry-After", out var retryAfterValues))
                {
                    if (int.TryParse(retryAfterValues.FirstOrDefault(), out int retryAfter))
                    {
                        _resetTime = DateTime.UtcNow.AddSeconds(retryAfter);
                        _remaining = 0;
                    }
                }
            }
        }
    }
}
