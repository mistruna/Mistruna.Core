using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Mistruna.Core.Microservices.Core.Infrastructure.Authorization;
using Mistruna.Core.Microservices.Core.JwtAuth;
using Mistruna.Core.Microservices.Core.JwtAuth.Policies;
using Swashbuckle.AspNetCore.Filters;

namespace Mistruna.Core.Microservices.Core.ServerMiddleware;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
    {
        return AddJwtAuthorizationCore(services, new JwtAuthorizationOptions
        {
            Issuer = AuthOptions.JwtIssuer,
            Audience = AuthOptions.JwtIssuer,
            Key = AuthOptions.JwtKey,
            KeyId = AuthOptions.JwtKeyId
        });
    }

    public static IServiceCollection AddJwtAuthorization(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<JwtAuthorizationOptions>? configure = null)
    {
        var options = new JwtAuthorizationOptions
        {
            Issuer = configuration["Jwt:Issuer"] ?? "Mistruna",
            Audience = configuration["Jwt:Audience"] ?? configuration["Jwt:Issuer"] ?? "Mistruna",
            Key = configuration["Jwt:Key"] ?? Environment.GetEnvironmentVariable("KEY"),
            KeyId = configuration["Jwt:KeyId"] ?? "mistruna-signing-key"
        };

        configure?.Invoke(options);

        if (string.IsNullOrWhiteSpace(options.Key) && options.AllowToolingFallbackKey)
        {
            options.Key = options.ToolingFallbackKey;
        }

        if (string.IsNullOrWhiteSpace(options.Key))
        {
            throw new InvalidOperationException(
                "JWT signing key is not configured. Set 'Jwt:Key' in configuration or 'KEY' environment variable.");
        }

        return AddJwtAuthorizationCore(services, options);
    }

    private static IServiceCollection AddJwtAuthorizationCore(
        IServiceCollection services,
        JwtAuthorizationOptions options)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key!))
                {
                    KeyId = options.KeyId
                };

                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = options.Issuer,
                    ValidAudience = options.Audience,
                    IssuerSigningKey = signingKey,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = options.RoleClaimType,
                    TryAllIssuerSigningKeys = true
                };
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.AdminOnly, policy => policy.RequireRole(RoleType.Administrator));
        });

        return services;
    }

    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection collection, string assemblyName)
    {
        collection.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v3", new OpenApiInfo
            {
                Title = assemblyName + " API",
                Version = "v3"
            });
            var baseDirectory = AppContext.BaseDirectory;
            c.IncludeXmlComments(baseDirectory + assemblyName + ".xml");
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. " +
                              "Example: \"Authorization: Bearer {token}\"",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT"
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        return collection;
    }
}
