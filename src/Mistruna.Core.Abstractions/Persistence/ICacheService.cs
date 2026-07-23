namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Provides asynchronous cache operations.</summary>
public interface ICacheService
{
    /// <summary>Gets a cached value.</summary>
    Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    /// <summary>Sets a cached value.</summary>
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
    /// <summary>Gets or creates a cached value.</summary>
    Task<T?> GetOrCreateAsync<T>(
        string key,
        Func<Task<T?>> factory,
        TimeSpan? expiration = null,
        CancellationToken cancellationToken = default);
    /// <summary>Removes a cached value.</summary>
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    /// <summary>Removes cached values matching a pattern.</summary>
    Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default);
    /// <summary>Checks whether a cache key exists.</summary>
    Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    /// <summary>Refreshes a cached value's expiration.</summary>
    Task RefreshAsync(string key, TimeSpan expiration, CancellationToken cancellationToken = default);
}

/// <summary>Configures cache behavior.</summary>
public class CacheOptions
{
    /// <summary>Gets or sets the default expiration.</summary>
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
    /// <summary>Gets or sets the cache key prefix.</summary>
    public string KeyPrefix { get; set; } = string.Empty;
    /// <summary>Gets or sets whether sliding expiration is used.</summary>
    public bool UseSlidingExpiration { get; set; }
}
