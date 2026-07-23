using System.Net.Mime;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Monetization.Authentication.ApiKey;
using Mistruna.Core.Monetization.Authorization.Plans;

namespace Mistruna.Core.Monetization.RateLimiting.Tiered;

/// <summary>
/// Middleware that enforces a per-API-key quota for the current calendar day (UTC) by
/// reading the <see cref="PlanClaimTypes.Quota"/> claim from the authenticated principal
/// and incrementing a counter via <see cref="IQuotaStore"/>.
/// </summary>
/// <remarks>
/// Requests without an <see cref="ApiKeyClaimTypes.ApiKeyId"/> claim (e.g., anonymous
/// public-page reads) bypass the limiter — IP-based limiting remains the job of
/// <c>RateLimitingMiddleware</c>.
/// </remarks>
public sealed class TieredRateLimitMiddleware(
    RequestDelegate next,
    TieredRateLimitOptions options,
    IQuotaStore store,
    ILogger<TieredRateLimitMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var apiKeyId = context.User.FindFirst(ApiKeyClaimTypes.ApiKeyId)?.Value;
        var quotaClaim = context.User.FindFirst(PlanClaimTypes.Quota)?.Value;

        if (apiKeyId is null || !long.TryParse(quotaClaim, out var quota))
        {
            await next(context);
            return;
        }

        var window = TimeSpan.FromSeconds(options.WindowSeconds);
        var key = $"{options.KeyPrefix}:{apiKeyId}:{DateTime.UtcNow:yyyyMMdd}";
        var result = await store.IncrementAsync(key, window, context.RequestAborted);

        if (options.IncludeHeaders)
        {
            context.Response.Headers["X-RateLimit-Limit"] = quota.ToString();
            context.Response.Headers["X-RateLimit-Remaining"] = Math.Max(0, quota - result.Count).ToString();
            context.Response.Headers["X-RateLimit-Reset"] = ((int)result.RetryAfter.TotalSeconds).ToString();
        }

        if (result.Count > quota)
        {
            logger.LogWarning(
                "Quota exceeded for ApiKey {ApiKeyId}. Count={Count}/{Quota}",
                apiKeyId, result.Count, quota);

            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            context.Response.ContentType = MediaTypeNames.Application.Json;
            context.Response.Headers["Retry-After"] = ((int)result.RetryAfter.TotalSeconds).ToString();

            var errorResponse = new
            {
                message = "Daily quota exceeded. Upgrade your plan or wait for the window reset.",
                errorCode = "QUOTA_EXCEEDED",
                retryAfterSeconds = (int)result.RetryAfter.TotalSeconds
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
            return;
        }

        await next(context);
    }
}
