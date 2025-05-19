using StackExchange.Redis;
using System.Collections.Concurrent;

namespace ChatService.TrackerRedis
{
    public class PresenceService
    {
        private readonly IDatabase _redis;

        public PresenceService(IConnectionMultiplexer redis)
        {
            _redis = redis.GetDatabase();
        }

        private string OnlineKey(string userId) => $"user:{userId}:online";
        private string LastSeenKey(string userId) => $"user:{userId}:lastseen";

        public async Task SetOnline(string userId)
        {
            await _redis.StringSetAsync(OnlineKey(userId), "1", TimeSpan.FromMinutes(10));
        }

        public async Task SetOffline(string userId)
        {
            await _redis.KeyDeleteAsync(OnlineKey(userId));
            await _redis.StringSetAsync(LastSeenKey(userId), DateTime.UtcNow.ToString("o"));
        }

        public async Task<bool> IsOnline(string userId)
        {
            return await _redis.KeyExistsAsync(OnlineKey(userId));
        }

        public async Task<DateTime?> GetLastSeen(string userId)
        {
            var val = await _redis.StringGetAsync(LastSeenKey(userId));
            return val.HasValue ? DateTime.Parse(val!) : null;
        }
    }


}
