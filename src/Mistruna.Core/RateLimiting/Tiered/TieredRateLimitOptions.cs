namespace Mistruna.Core.RateLimiting.Tiered;

/// <summary>
/// Configuration options for tiered rate limiting middleware.
/// </summary>
public sealed class TieredRateLimitOptions
{
    /// <summary>Rolling window size, in seconds. Defaults to 86,400 (daily quota).</summary>
    public int WindowSeconds { get; set; } = 86_400;

    /// <summary>Redis-key prefix (or in-memory key prefix). Defaults to <c>quota</c>.</summary>
    public string KeyPrefix { get; set; } = "quota";

    /// <summary>Whether to emit <c>X-RateLimit-*</c> response headers. Defaults to true.</summary>
    public bool IncludeHeaders { get; set; } = true;
}
