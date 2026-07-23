using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Mistruna.Core.Monetization.RateLimiting.Tiered;

/// <summary>
/// Redis-backed counter store. Uses <c>INCR</c> with a TTL attached on the first write of the
/// window. On Redis failure, throws — callers should wrap in a policy or fall back via
/// composition to <see cref="InMemoryQuotaStore"/>.
/// </summary>
public sealed class RedisQuotaStore(
    IConnectionMultiplexer redis,
    ILogger<RedisQuotaStore> logger) : IQuotaStore
{
    public async Task<QuotaResult> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken)
    {
        try
        {
            var db = redis.GetDatabase();
            var count = await db.StringIncrementAsync(key);

            if (count == 1)
            {
                await db.KeyExpireAsync(key, window);
            }

            var ttl = await db.KeyTimeToLiveAsync(key);
            return new QuotaResult(count, ttl ?? window);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis INCR failed for key {Key}", key);
            throw;
        }
    }
}
