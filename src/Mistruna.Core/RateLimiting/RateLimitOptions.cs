namespace Mistruna.Core.RateLimiting;

/// <summary>
/// Configuration options for the rate limiting middleware.
/// </summary>
public class RateLimitOptions
{
    /// <summary>
    /// Gets or sets the maximum number of requests allowed per time window.
    /// Defaults to 100.
    /// </summary>
    public int RequestsPerWindow { get; set; } = 100;

    /// <summary>
    /// Gets or sets the time window duration in seconds.
    /// Defaults to 60 seconds.
    /// </summary>
    public int WindowSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets the Redis key prefix for rate limit counters.
    /// Defaults to "ratelimit".
    /// </summary>
    public string KeyPrefix { get; set; } = "ratelimit";

    /// <summary>
    /// Gets or sets a value indicating whether to include rate limit headers in responses.
    /// Defaults to true.
    /// </summary>
    public bool IncludeHeaders { get; set; } = true;
}
