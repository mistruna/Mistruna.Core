namespace Mistruna.Core.Authentication.ApiKey;

/// <summary>
/// Claim type constants set by <see cref="ApiKeyAuthenticationHandler"/> on the principal.
/// The `sub` claim is the API key's owning user ID (Guid).
/// </summary>
public static class ApiKeyClaimTypes
{
    public const string Subject = "sub";
    public const string ApiKeyId = "apikey_id";
}
