using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Mistruna.Core.AspNetCore.Authentication;

public static class JwtServiceCollectionExtensions
{
    public static IServiceCollection AddMistrunaJwtAuthorization(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<MistrunaJwtAuthorizationOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var options = new MistrunaJwtAuthorizationOptions
        {
            Issuer = configuration["Jwt:Issuer"] ?? "Mistruna",
            Audience = configuration["Jwt:Audience"] ?? configuration["Jwt:Issuer"] ?? "Mistruna",
            Key = configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("KEY"),
            KeyId = configuration["Jwt:KeyId"] ?? "mistruna-signing-key"
        };

        configure?.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Key))
        {
            throw new InvalidOperationException(
                "JWT signing key is not configured. Set 'Jwt:Key' or the 'KEY' environment variable.");
        }

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key))
        {
            KeyId = options.KeyId
        };

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(jwt =>
            {
                jwt.RequireHttpsMetadata = options.RequireHttpsMetadata;
                jwt.SaveToken = true;
                jwt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = options.RoleClaimType,
                    TryAllIssuerSigningKeys = true
                };
            });

        services.AddAuthorization(authorization =>
            authorization.AddPolicy(
                MistrunaAuthorizationPolicies.AdminOnly,
                policy => policy.RequireRole(MistrunaRoles.Administrator)));
        return services;
    }
}
