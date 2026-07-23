namespace Mistruna.Core.Monetization.RateLimiting.Tiered;

/// <summary>
/// Outcome of an <see cref="IQuotaStore.IncrementAsync"/> call.
/// </summary>
/// <param name="Count">Current counter value after the increment.</param>
/// <param name="RetryAfter">Time until the counter resets (for Redis this is the remaining TTL).</param>
public readonly record struct QuotaResult(long Count, TimeSpan RetryAfter);
