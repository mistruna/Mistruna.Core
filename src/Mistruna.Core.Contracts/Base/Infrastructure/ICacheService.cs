namespace Mistruna.Core.Contracts.Base.Infrastructure;

/// <summary>
/// Represents a cache service for storing and retrieving cached data.
/// </summary>
/// <remarks>
/// Implement this interface to provide distributed or in-memory caching capabilities.
/// </remarks>
/// <example>
/// <code>
/// // Usage with Redis or Memory Cache
/// var user = await cacheService.GetOrCreateAsync(
///     $"user:{userId}",
///     async () => await userRepository.GetByIdAsync(userId),
///     TimeSpan.FromMinutes(10));
/// </code>
/// </example>
public interface ICacheService
{
    /// <summary>
    /// Gets a value from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The cached value or default if not found.</returns>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sets a value in the cache.
    /// </summary>
    /// <typeparam name="T">The type of the value to cache.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="expiration">The expiration time.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task SetAsync<T>(
        string key,
        T value,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a value from the cache or creates it if it doesn't exist.
    /// </summary>
    /// <typeparam name="T">The type of the cached value.</typeparam>
    /// <param name="key">The cache key.</param>
    /// <param name="factory">The factory function to create the value if not found.</param>
    /// <param name="expiration">The expiration time.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The cached or newly created value.</returns>
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a value from the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all values matching a pattern from the cache.
    /// </summary>
    /// <param name="pattern">The pattern to match (e.g., "user:*").</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>True if the key exists.</returns>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Refreshes the expiration time of a cached item.
    /// </summary>
    /// <param name="key">The cache key.</param>
    /// <param name="expiration">The new expiration time.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RefreshAsync(
        string key,
        TimeSpan expiration,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Cache options for configuring cache behavior.
/// </summary>
public class CacheOptions
{
    /// <summary>
    /// Gets or sets the default expiration time.
    /// </summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets the cache key prefix.
    /// </summary>
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets a value indicating whether to use sliding expiration.
    /// </summary>
    public bool UseSlidingExpiration { get; set; }
}
