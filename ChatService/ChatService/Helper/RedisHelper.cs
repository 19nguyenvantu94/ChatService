using StackExchange.Redis;

namespace ChatService.Helper
{
    public static class RedisHelper
    {
        private static ConnectionMultiplexer _redis;
        private static IDatabase _db;
        //private readonly ILogger<RedisHelper> _logger;

        private static bool _redisAvailable = false;

        public static async Task InitAsync(string connectionString)
        {
            try
            {
                _redis = await ConnectionMultiplexer.ConnectAsync(connectionString);
                _db = _redis.GetDatabase();
                _redisAvailable = true;
                //_logger.LogInformation("🔗 Redis connected!");
            }
            catch (Exception ex)
            {
                _redisAvailable = false;
                //_logger.LogInformation($"⚠️ Redis connection failed: {ex.Message}");
            }
        }

        public static async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (!_redisAvailable) return false;
            try
            {
                return await _db.StringSetAsync(key, value, expiry);
            }
            catch
            {
                //_logger.LogInformation("Redis SET failed");
                //Console.WriteLine("⚠️ Redis SET failed.");
                return false;
            }
        }

        public static async Task<string> GetAsync(string key)
        {
            if (!_redisAvailable) return null;
            try
            {
                return await _db.StringGetAsync(key);
            }
            catch
            {
                //_logger.LogInformation("Redis GET failed.");
                return null;
            }
        }
    }
}
