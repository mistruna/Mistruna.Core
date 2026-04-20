using System.Text.Json;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Mistruna.Core.Idempotency;

/// <summary>
/// Redis-backed store. Serializes <see cref="IdempotentResponse"/> as JSON and stores under
/// the provided key with the supplied TTL.
/// </summary>
public sealed class RedisIdempotencyStore(
    IConnectionMultiplexer redis,
    ILogger<RedisIdempotencyStore> logger) : IIdempotencyStore
{
    private static readonly JsonSerializerOptions Json = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<IdempotentResponse?> GetAsync(string key, CancellationToken cancellationToken)
    {
        try
        {
            var value = await redis.GetDatabase().StringGetAsync(key);
            if (!value.HasValue) return null;
            return JsonSerializer.Deserialize<IdempotentResponse>((string)value!, Json);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis GET failed for idempotency key {Key}", key);
            return null;
        }
    }

    public async Task SetAsync(string key, IdempotentResponse response, TimeSpan ttl, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(response, Json);
            await redis.GetDatabase().StringSetAsync(key, json, ttl);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis SET failed for idempotency key {Key}", key);
        }
    }
}
