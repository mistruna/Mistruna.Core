namespace Mistruna.Core.AspNetCore.RateLimiting;

public sealed class MistrunaIpRateLimitOptions
{
    public int RequestsPerWindow { get; set; } = 100;

    public int WindowSeconds { get; set; } = 60;

    public string KeyPrefix { get; set; } = "mistruna:ratelimit";

    public bool IncludeHeaders { get; set; } = true;
}
