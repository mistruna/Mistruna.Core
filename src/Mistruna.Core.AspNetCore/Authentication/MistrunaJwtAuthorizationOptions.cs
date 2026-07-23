namespace Mistruna.Core.AspNetCore.Authentication;

public static class MistrunaAuthorizationPolicies
{
    public const string AdminOnly = nameof(AdminOnly);
}

public static class MistrunaRoles
{
    public const string Administrator = nameof(Administrator);
}

public sealed class MistrunaJwtAuthorizationOptions
{
    public string Issuer { get; set; } = "Mistruna";

    public string Audience { get; set; } = "Mistruna";

    public string? Key { get; set; }

    public string? KeyId { get; set; } = "mistruna-signing-key";

    public string RoleClaimType { get; set; } = "role";

    public bool RequireHttpsMetadata { get; set; } = true;
}
