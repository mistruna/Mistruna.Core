using Microsoft.AspNetCore.Authorization;

namespace Mistruna.Core.Authorization.Plans;

/// <summary>
/// Requirement that the current principal's <see cref="PlanClaimTypes.Tier"/> claim
/// is &gt;= <see cref="RequiredPlan"/>.
/// </summary>
public sealed class RequiresPlanRequirement(Plan requiredPlan) : IAuthorizationRequirement
{
    public Plan RequiredPlan { get; } = requiredPlan;
}
