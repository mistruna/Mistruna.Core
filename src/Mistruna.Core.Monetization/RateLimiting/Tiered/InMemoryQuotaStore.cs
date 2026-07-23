using System.Collections.Concurrent;

namespace Mistruna.Core.Monetization.RateLimiting.Tiered;

/// <summary>
/// In-process counter store. Used as a fallback when Redis is unavailable and as a
/// test double. Does not survive horizontal scaling or pod restarts.
/// </summary>
public sealed class InMemoryQuotaStore : IQuotaStore
{
    private readonly ConcurrentDictionary<string, (long Count, DateTime WindowStart, TimeSpan Window)> _counters = new();

    public Task<QuotaResult> IncrementAsync(string key, TimeSpan window, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var entry = _counters.AddOrUpdate(
            key,
            _ => (Count: 1L, WindowStart: now, Window: window),
            (_, existing) => now - existing.WindowStart >= existing.Window
                ? (Count: 1L, WindowStart: now, Window: window)
                : (Count: existing.Count + 1, existing.WindowStart, existing.Window));

        var elapsed = now - entry.WindowStart;
        var remaining = entry.Window - elapsed;
        if (remaining < TimeSpan.Zero)
            remaining = TimeSpan.Zero;

        return Task.FromResult(new QuotaResult(entry.Count, remaining));
    }
}
