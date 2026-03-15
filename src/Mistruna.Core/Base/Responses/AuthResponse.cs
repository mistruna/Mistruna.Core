namespace Mistruna.Core.Base.Responses;

/// <summary>
/// Standard authentication response of Create, Update, Get single operation
/// </summary>
public class AuthResponse
{
    /// <summary>Username</summary>
    public string Username { get; set; }

    /// <summary>Email</summary>
    public string Email { get; set; }

    /// <summary>Token</summary>
    public string Token { get; set; }

    /// <summary>Refresh token</summary>
    public string RefreshToken { get; set; }
}
