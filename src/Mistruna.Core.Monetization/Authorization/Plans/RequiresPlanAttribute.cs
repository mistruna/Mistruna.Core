using Microsoft.AspNetCore.Authorization;

namespace Mistruna.Core.Monetization.Authorization.Plans;

/// <summary>
/// Declares that an MVC controller or action requires the caller's subscription tier
/// to be &gt;= the specified <see cref="Plan"/>. On failure, pipeline returns 402 Payment Required
/// (see <see cref="RequiresPlanAuthorizationHandler"/>).
/// </summary>
/// <example>
/// <code>
/// [RequiresPlan(Plan.Pro)]
/// [HttpPost("/api/alerts")]
/// public Task&lt;IActionResult&gt; CreateAlert(...) { ... }
/// </code>
/// </example>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
public sealed class RequiresPlanAttribute : AuthorizeAttribute
{
    public RequiresPlanAttribute(Plan plan)
    {
        Plan = plan;
        Policy = BuildPolicyName(plan);
    }

    public Plan Plan { get; }

    /// <summary>
    /// Deterministic policy name so <see cref="RequiresPlanPolicyProvider"/> can
    /// round-trip attribute &lt;-&gt; policy.
    /// </summary>
    public static string BuildPolicyName(Plan plan) => $"RequiresPlan:{plan}";
}
