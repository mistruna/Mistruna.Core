namespace Mistruna.Core.Monetization.RateLimiting.Tiered;

/// <summary>
/// Atomic counter store used by <see cref="TieredRateLimitMiddleware"/>.
/// Implementations must guarantee that concurrent <see cref="IncrementAsync"/> calls on the
/// same key return strictly monotonically increasing values (Redis <c>INCR</c> is naturally
/// atomic; in-memory uses a lock).
/// </summary>
public interface IQuotaStore
{
    /// <summary>
    /// Increments the counter at <paramref name="key"/>. On first call, sets a TTL equal to
    /// <paramref name="window"/> (so quota resets automatically). Returns the post-increment
    /// count and the remaining TTL.
    /// </summary>
    Task<QuotaResult> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken);
}
