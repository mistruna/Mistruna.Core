using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;

namespace Mistruna.Core.Monetization.Authorization.Plans;

/// <summary>
/// Translates a failed <see cref="RequiresPlanRequirement"/> into an HTTP 402
/// Payment Required response (rather than the default 403 Forbidden) so that UI
/// clients can distinguish "you need to upgrade your plan" from "this resource is
/// forbidden to you regardless of plan". Unrelated authorization failures fall
/// through to the default <see cref="AuthorizationMiddlewareResultHandler"/>.
/// </summary>
public sealed class PlanAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private readonly AuthorizationMiddlewareResultHandler _fallback = new();

    public async Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (!authorizeResult.Succeeded
            && authorizeResult.AuthorizationFailure?.FailedRequirements
                .OfType<RequiresPlanRequirement>()
                .FirstOrDefault() is { } planRequirement)
        {
            context.Response.StatusCode = StatusCodes.Status402PaymentRequired;
            context.Response.ContentType = "application/json";

            var payload = JsonSerializer.Serialize(new
            {
                message = "Upgrade required to access this resource.",
                errorCode = "UPGRADE_REQUIRED",
                requiredPlan = planRequirement.RequiredPlan.ToString(),
                traceId = context.TraceIdentifier,
                timestamp = DateTime.UtcNow
            });
            await context.Response.WriteAsync(payload);
            return;
        }

        // For forbid results not caused by a plan requirement, return 403 directly.
        // For all other results (success, challenge/unauthenticated), delegate to the
        // default handler which correctly handles redirects and challenge responses.
        if (authorizeResult.Forbidden)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        await _fallback.HandleAsync(next, context, policy, authorizeResult);
    }
}
