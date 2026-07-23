using System.Collections.Concurrent;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mistruna.Core.Abstractions.Persistence;
using StackExchange.Redis;

namespace Mistruna.Core.Caching.Redis;

/// <summary>
/// Redis-backed implementation of <see cref="ICacheService"/>.
/// Uses StackExchange.Redis for distributed caching with JSON serialization.
/// Includes double-check locking in <see cref="GetOrCreateAsync{T}"/> to prevent cache stampede.
/// </summary>
public sealed class RedisCacheService(
    IConnectionMultiplexer connection,
    ILogger<RedisCacheService> logger,
    IOptions<CacheOptions> options) : ICacheService
{
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> Locks = new();
    private readonly IDatabase _database = connection.GetDatabase();
    private readonly CacheOptions _options = options.Value;
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <inheritdoc />
    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        var prefixedKey = GetPrefixedKey(key);

        try
        {
            var value = await _database.StringGetAsync(prefixedKey);

            if (!value.HasValue)
                return default;

            if (_options.UseSlidingExpiration)
            {
                await _database.KeyExpireAsync(prefixedKey, _options.DefaultExpiration);
            }

            return JsonSerializer.Deserialize<T>((string)value!, _jsonOptions);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis GET failed for key {Key}", prefixedKey);
            return default;
        }
    }

    /// <inheritdoc />
    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var prefixedKey = GetPrefixedKey(key);
        var ttl = expiration ?? _options.DefaultExpiration;

        try
        {
            var serialized = JsonSerializer.Serialize(value, _jsonOptions);
            await _database.StringSetAsync(prefixedKey, serialized, ttl);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis SET failed for key {Key}", prefixedKey);
        }
    }

    /// <inheritdoc />
    public async Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default)
    {
        var existing = await GetAsync<T>(key, cancellationToken);
        if (existing is not null)
            return existing;

        var lockKey = GetPrefixedKey(key);
        var semaphore = Locks.GetOrAdd(lockKey, _ => new SemaphoreSlim(1, 1));

        await semaphore.WaitAsync(cancellationToken);
        try
        {
            existing = await GetAsync<T>(key, cancellationToken);
            if (existing is not null)
            {
                return existing;
            }

            var value = await factory();
            if (value is not null)
            {
                await SetAsync(key, value, expiration, cancellationToken);
            }

            return value;
        }
        finally
        {
            semaphore.Release();
        }
    }

    /// <inheritdoc />
    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        var prefixedKey = GetPrefixedKey(key);

        try
        {
            await _database.KeyDeleteAsync(prefixedKey);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis DELETE failed for key {Key}", prefixedKey);
        }
    }

    /// <inheritdoc />
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var prefixedPattern = GetPrefixedKey(pattern);

        try
        {
            foreach (var endpoint in connection.GetEndPoints())
            {
                var server = connection.GetServer(endpoint);
                foreach (var key in server.Keys(pattern: prefixedPattern))
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis REMOVE BY PATTERN failed for pattern {Pattern}", prefixedPattern);
        }
    }

    /// <inheritdoc />
    public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
    {
        var prefixedKey = GetPrefixedKey(key);

        try
        {
            return await _database.KeyExistsAsync(prefixedKey);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis EXISTS failed for key {Key}", prefixedKey);
            return false;
        }
    }

    /// <inheritdoc />
    public async Task RefreshAsync(
        string key,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var prefixedKey = GetPrefixedKey(key);

        try
        {
            await _database.KeyExpireAsync(prefixedKey, expiration);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis REFRESH failed for key {Key}", prefixedKey);
        }
    }

    private string GetPrefixedKey(string key) =>
        string.IsNullOrEmpty(_options.KeyPrefix) ? key : $"{_options.KeyPrefix}:{key}";
}
