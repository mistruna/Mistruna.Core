using System.Collections.Concurrent;
using System.Net.Mime;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Mistruna.Core.RateLimiting;

/// <summary>
/// Middleware that enforces per-IP rate limiting using Redis for distributed counter storage,
/// with an in-memory <see cref="ConcurrentDictionary{TKey,TValue}"/> fallback when Redis is unavailable.
/// Returns HTTP 429 Too Many Requests when the limit is exceeded.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RateLimitingMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
/// <param name="options">Rate limiting configuration options.</param>
/// <param name="logger">The logger instance.</param>
/// <param name="redis">Optional Redis connection multiplexer. Falls back to in-memory if null.</param>
public class RateLimitingMiddleware(
    RequestDelegate next,
    RateLimitOptions options,
    ILogger<RateLimitingMiddleware> logger,
    IConnectionMultiplexer? redis = null)
{
    // In-memory fallback when Redis is unavailable
    private readonly ConcurrentDictionary<string, (int Count, DateTime WindowStart)> _inMemoryCounters = new();

    /// <summary>
    /// Processes the HTTP request and enforces rate limiting.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    public async Task InvokeAsync(HttpContext context)
    {
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"{options.KeyPrefix}:{clientIp}";
        var window = TimeSpan.FromSeconds(options.WindowSeconds);

        var (requestCount, remaining, retryAfter) = redis is { IsConnected: true }
            ? await CheckRateLimitRedisAsync(key, window)
            : CheckRateLimitInMemory(key, window);

        if (options.IncludeHeaders)
        {
            context.Response.Headers["X-RateLimit-Limit"] = options.RequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, remaining).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = retryAfter.ToString();
        }

        if (requestCount > options.RequestsPerWindow)
        {
            logger.LogWarning("Rate limit exceeded for {ClientIp}. Count: {Count}/{Limit}",
                clientIp, requestCount, options.RequestsPerWindow);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            if (options.IncludeHeaders)
            {
                context.Response.Headers.RetryAfter = retryAfter.ToString();
            }

            await context.Response.WriteAsJsonAsync(new
            {
                message = "Too many requests. Please try again later.",
                errorCode = "RATE_LIMIT_EXCEEDED",
                retryAfterSeconds = retryAfter
            });

            return;
        }

        await next(context);
    }

    private async Task<(long Count, long Remaining, long RetryAfter)> CheckRateLimitRedisAsync(
        string key, TimeSpan window)
    {
        try
        {
            var db = redis!.GetDatabase();
            var count = await db.StringIncrementAsync(key);

            if (count == 1)
            {
                await db.KeyExpireAsync(key, window);
            }

            var ttl = await db.KeyTimeToLiveAsync(key);
            var retryAfter = (long)(ttl?.TotalSeconds ?? options.WindowSeconds);
            var remaining = options.RequestsPerWindow - count;

            return (count, remaining, retryAfter);
        }
        catch (RedisException ex)
        {
            logger.LogError(ex, "Redis rate limit check failed, falling back to in-memory");
            return CheckRateLimitInMemory(key, window);
        }
    }

    private (long Count, long Remaining, long RetryAfter) CheckRateLimitInMemory(
        string key, TimeSpan window)
    {
        var now = DateTime.UtcNow;

        var entry = _inMemoryCounters.AddOrUpdate(
            key,
            _ => (1, now),
            (_, existing) =>
            {
                if (now - existing.WindowStart >= window)
                    return (1, now);

                return (existing.Count + 1, existing.WindowStart);
            });

        var elapsed = now - entry.WindowStart;
        var retryAfter = (long)Math.Max(0, window.TotalSeconds - elapsed.TotalSeconds);
        var remaining = options.RequestsPerWindow - entry.Count;

        return (entry.Count, remaining, retryAfter);
    }
}
