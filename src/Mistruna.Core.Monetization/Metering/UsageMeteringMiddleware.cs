using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Monetization.Authentication.ApiKey;

namespace Mistruna.Core.Monetization.Metering;

/// <summary>
/// Records a usage event for the current API key after the pipeline produces a successful
/// response. Never blocks or throws into the pipeline: the meter call is wrapped in a
/// try/catch, and failures log-and-continue (see spec § 5.5).
/// </summary>
public sealed class UsageMeteringMiddleware(
    RequestDelegate next,
    UsageMeteringOptions options,
    IUsageMeter meter,
    ILogger<UsageMeteringMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await next(context);

        var apiKeyIdClaim = context.User.FindFirst(ApiKeyClaimTypes.ApiKeyId)?.Value;
        if (apiKeyIdClaim is null || !Guid.TryParse(apiKeyIdClaim, out var apiKeyId))
        {
            return;
        }

        if (!options.IsSuccessful(context.Response.StatusCode))
        {
            return;
        }

        try
        {
            await meter.IncrementAsync(apiKeyId, DateOnly.FromDateTime(DateTime.UtcNow), context.RequestAborted);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Usage metering failed for {ApiKeyId}", apiKeyId);
        }
    }
}
