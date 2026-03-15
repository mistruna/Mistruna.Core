using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Mistruna.Core.Base.ComplexTypes;
using Mistruna.Core.Microservices.Core.Infrastructure.Authorization;
using Mistruna.Core.Microservices.Core.JwtAuth.Entities;

namespace Mistruna.Core.Microservices.Core.JwtAuth;

public static class JwtTokenProvider
{
    private static string GenerateJwtToken(UserEntity user, IEnumerable<IdentityRole<Guid>> roles)
    {
        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString()),
            new("Username", user.UserName),
            new("Email", user.Email),
            new("Role", string.Join(" ", roles.Select(x => x.Name))),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.JwtKey));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.Now.AddDays(30);

        var token = new JwtSecurityToken(
            issuer: "Mistruna",
            audience: "Mistruna",
            claims: claims,
            notBefore: null,
            expires: expires,
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public static string GenerateSystemToken() => GenerateSystemToken(SystemUsersEnumeration.SystemMessage);

    public static string GenerateSystemToken(SystemUsersEnumeration user)
    {
        var systemUser = new UserEntity
        {
            Id = Guid.NewGuid(),
            UserName = "System",
            Email = "system@mistruna",
            FirstName = "System",
            LastName = "System",
            MiddleName = "System",
            DateOfBirth = DateTime.UtcNow.AddYears(-30),
            Country = "System",
            City = "System",
            Language = "System",
            Agreement = true
        };

        return GenerateJwtToken(systemUser, []);
    }
}
