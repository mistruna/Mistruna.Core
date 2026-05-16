namespace Mistruna.Core.Microservices.Core.JwtAuth;

public sealed class JwtAuthorizationOptions
{
    public string Issuer { get; set; } = "Mistruna";

    public string Audience { get; set; } = "Mistruna";

    public string? Key { get; set; }

    public string? KeyId { get; set; } = "mistruna-signing-key";

    public string RoleClaimType { get; set; } = "role";

    public bool AllowToolingFallbackKey { get; set; }

    public string ToolingFallbackKey { get; set; } = "00000000-0000-0000-0000-000000000000";
}
