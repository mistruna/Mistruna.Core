namespace Mistruna.Core.Authentication.ApiKey;

/// <summary>
/// Default constants for the API key authentication scheme.
/// </summary>
public static class ApiKeyDefaults
{
    /// <summary>The authentication scheme name registered in DI.</summary>
    public const string AuthenticationScheme = "ApiKey";

    /// <summary>The default HTTP header name carrying the API key.</summary>
    public const string HeaderName = "X-Api-Key";
}
