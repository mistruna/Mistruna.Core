using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Mistruna.Core.Microservices.Core.Infrastructure.Authorization;

namespace Mistruna.Core.Microservices.Core.JwtAuth;

/// <summary>
/// Generates signed JWT tokens from arbitrary claims.
/// Each microservice builds its own claims list from its domain entities
/// and passes them here — this class has no domain coupling.
/// </summary>
public static class JwtTokenProvider
{
    /// <summary>
    /// Generates a signed JWT token for the given claims.
    /// Uses <see cref="AuthOptions.JwtKey"/>, <see cref="AuthOptions.JwtKeyId"/>,
    /// and <see cref="AuthOptions.JwtIssuer"/> from static configuration.
    /// </summary>
    /// <param name="claims">Domain-specific claims to embed in the token.</param>
    /// <param name="expiry">Token expiry (default: 30 days from now UTC).</param>
    public static string GenerateToken(IEnumerable<Claim> claims, DateTime? expiry = null)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.JwtKey))
        {
            KeyId = AuthOptions.JwtKeyId
        };

        var allClaims = claims
            .Append(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()))
            .Append(new Claim(JwtRegisteredClaimNames.Iat,
                DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()));

        var token = new JwtSecurityToken(
            issuer: AuthOptions.JwtIssuer,
            audience: AuthOptions.JwtIssuer,
            claims: allClaims,
            notBefore: null,
            expires: expiry ?? DateTime.UtcNow.AddDays(30),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    /// <summary>
    /// Generates a system-level service-to-service token.
    /// </summary>
    public static string GenerateSystemToken()
    {
        var claims = new[]
        {
            new Claim("UserId", Guid.Empty.ToString()),
            new Claim("Username", "system"),
            new Claim("Role", "System")
        };

        return GenerateToken(claims, DateTime.UtcNow.AddDays(365));
    }
}
