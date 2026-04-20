using Microsoft.AspNetCore.Authorization;

namespace Mistruna.Core.Authorization.Plans;

/// <summary>
/// Evaluates <see cref="RequiresPlanRequirement"/> by reading the
/// <see cref="PlanClaimTypes.Tier"/> claim and comparing it numerically to the requirement.
/// A missing or unparseable claim fails the requirement — we fail closed so callers
/// without a tier claim cannot accidentally access gated endpoints.
/// </summary>
public sealed class RequiresPlanAuthorizationHandler : AuthorizationHandler<RequiresPlanRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequiresPlanRequirement requirement)
    {
        var tierClaim = context.User.FindFirst(PlanClaimTypes.Tier)?.Value;

        if (Enum.TryParse<Plan>(tierClaim, ignoreCase: true, out var currentPlan)
            && currentPlan >= requirement.RequiredPlan)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
