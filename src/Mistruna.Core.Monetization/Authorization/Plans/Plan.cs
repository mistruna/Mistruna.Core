namespace Mistruna.Core.Monetization.Authorization.Plans;

/// <summary>
/// Subscription tiers in ascending order of capability.
/// The numeric ordering is load-bearing: <see cref="RequiresPlanAuthorizationHandler"/> compares
/// tiers using the underlying integer values.
/// </summary>
public enum Plan
{
    Free = 0,
    Developer = 1,
    Pro = 2,
    Enterprise = 3
}
