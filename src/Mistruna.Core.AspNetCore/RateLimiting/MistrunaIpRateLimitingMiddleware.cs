using System.Collections.Concurrent;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace Mistruna.Core.AspNetCore.RateLimiting;

internal sealed class MistrunaIpRateLimitingMiddleware(
    RequestDelegate next,
    IOptions<MistrunaIpRateLimitOptions> options,
    ILogger<MistrunaIpRateLimitingMiddleware> logger,
    IConnectionMultiplexer? redis = null)
{
    private readonly ConcurrentDictionary<string, (int Count, DateTimeOffset WindowStart)> _counters = new();

    public async Task InvokeAsync(HttpContext context)
    {
        var settings = options.Value;
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var key = $"{settings.KeyPrefix}:{clientIp}";
        var window = TimeSpan.FromSeconds(settings.WindowSeconds);

        var result = redis is { IsConnected: true }
            ? await CheckRedisAsync(key, window, settings, redis)
            : CheckMemory(key, window, settings);

        if (settings.IncludeHeaders)
        {
            context.Response.Headers["X-RateLimit-Limit"] = settings.RequestsPerWindow.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, result.Remaining).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = result.RetryAfter.ToString();
        }

        if (result.Count <= settings.RequestsPerWindow)
        {
            await next(context);
            return;
        }

        logger.LogWarning("IP rate limit exceeded for {ClientIp}", clientIp);
        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        context.Response.ContentType = "application/problem+json";
        context.Response.Headers.RetryAfter = result.RetryAfter.ToString();

        var problem = new ProblemDetails
        {
            Status = StatusCodes.Status429TooManyRequests,
            Title = "Too many requests",
            Detail = "Please try again later."
        };
        problem.Extensions["errorCode"] = "RATE_LIMIT_EXCEEDED";
        problem.Extensions["traceId"] = context.TraceIdentifier;
        problem.Extensions["retryAfterSeconds"] = result.RetryAfter;

        await context.Response.WriteAsJsonAsync(problem, context.RequestAborted);
    }

    private async Task<RateLimitResult> CheckRedisAsync(
        string key,
        TimeSpan window,
        MistrunaIpRateLimitOptions settings,
        IConnectionMultiplexer connection)
    {
        try
        {
            var database = connection.GetDatabase();
            var count = await database.StringIncrementAsync(key);
            if (count == 1)
            {
                await database.KeyExpireAsync(key, window);
            }

            var ttl = await database.KeyTimeToLiveAsync(key);
            var retryAfter = Math.Max(0, (long)(ttl?.TotalSeconds ?? settings.WindowSeconds));
            return new(count, settings.RequestsPerWindow - count, retryAfter);
        }
        catch (RedisException exception)
        {
            logger.LogWarning(exception, "Redis rate limit check failed; using in-memory counters");
            return CheckMemory(key, window, settings);
        }
    }

    private RateLimitResult CheckMemory(
        string key,
        TimeSpan window,
        MistrunaIpRateLimitOptions settings)
    {
        var now = DateTimeOffset.UtcNow;
        var entry = _counters.AddOrUpdate(
            key,
            _ => (1, now),
            (_, current) => now - current.WindowStart >= window
                ? (1, now)
                : (current.Count + 1, current.WindowStart));

        var retryAfter = Math.Max(0, (long)(window - (now - entry.WindowStart)).TotalSeconds);
        return new(entry.Count, settings.RequestsPerWindow - entry.Count, retryAfter);
    }

    private readonly record struct RateLimitResult(long Count, long Remaining, long RetryAfter);
}
