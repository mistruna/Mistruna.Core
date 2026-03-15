using System.Text.Json;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Contracts.Base.Infrastructure;
using StackExchange.Redis;

namespace Mistruna.Core.Caching;

/// <summary>
/// Redis-backed implementation of <see cref="ICacheService"/>.
/// Uses StackExchange.Redis for distributed caching with JSON serialization.
/// </summary>
public sealed class RedisCacheService : ICacheService, IDisposable
{
    private readonly IConnectionMultiplexer _connection;
    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheService> _logger;
    private readonly CacheOptions _options;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="RedisCacheService"/> class.
    /// </summary>
    /// <param name="connection">The Redis connection multiplexer.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">Cache configuration options.</param>
    public RedisCacheService(
        IConnectionMultiplexer connection,
        ILogger<RedisCacheService> logger,
        CacheOptions? options = null)
    {
        _connection = connection;
        _database = connection.GetDatabase();
        _logger = logger;
        _options = options ?? new CacheOptions();
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

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
            _logger.LogError(ex, "Redis GET failed for key {Key}", prefixedKey);
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
            _logger.LogError(ex, "Redis SET failed for key {Key}", prefixedKey);
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

        var value = await factory();

        if (value is not null)
        {
            await SetAsync(key, value, expiration, cancellationToken);
        }

        return value;
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
            _logger.LogError(ex, "Redis DELETE failed for key {Key}", prefixedKey);
        }
    }

    /// <inheritdoc />
    public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        var prefixedPattern = GetPrefixedKey(pattern);

        try
        {
            foreach (var endpoint in _connection.GetEndPoints())
            {
                var server = _connection.GetServer(endpoint);
                var keys = server.Keys(pattern: prefixedPattern);

                foreach (var key in keys)
                {
                    await _database.KeyDeleteAsync(key);
                }
            }
        }
        catch (RedisException ex)
        {
            _logger.LogError(ex, "Redis REMOVE BY PATTERN failed for pattern {Pattern}", prefixedPattern);
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
            _logger.LogError(ex, "Redis EXISTS failed for key {Key}", prefixedKey);
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
            _logger.LogError(ex, "Redis REFRESH failed for key {Key}", prefixedKey);
        }
    }

    /// <summary>
    /// Disposes the Redis connection.
    /// </summary>
    public void Dispose()
    {
        _connection.Dispose();
    }

    private string GetPrefixedKey(string key) =>
        string.IsNullOrEmpty(_options.KeyPrefix) ? key : $"{_options.KeyPrefix}:{key}";
}
