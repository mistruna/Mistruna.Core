using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Mistruna.Core.Metering;

/// <summary>
/// Redis-backed meter. Counter key format: <c>usage:{apiKeyId:N}:{yyyyMMdd}</c>.
/// Keys have no TTL by default — downstream rollup jobs persist the counters into
/// Postgres and DELETE the Redis keys.
/// </summary>
public sealed class RedisUsageMeter(
    IConnectionMultiplexer redis,
    ILogger<RedisUsageMeter> logger) : IUsageMeter
{
    public async Task IncrementAsync(Guid apiKeyId, DateOnly date, CancellationToken cancellationToken)
    {
        try
        {
            var key = $"usage:{apiKeyId:N}:{date:yyyyMMdd}";
            await redis.GetDatabase().StringIncrementAsync(key);
        }
        catch (RedisException ex)
        {
            logger.LogWarning(ex, "Failed to increment usage counter for {ApiKeyId}", apiKeyId);
        }
    }
}
