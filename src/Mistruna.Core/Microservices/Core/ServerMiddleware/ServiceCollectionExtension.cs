using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mistruna.Core.Microservices.Core.Infrastructure.Authorization;
using Mistruna.Core.Microservices.Core.JwtAuth.Entities;
using Mistruna.Core.Microservices.Core.JwtAuth.Policies;
using Swashbuckle.AspNetCore.Filters;

namespace Mistruna.Core.Microservices.Core.ServerMiddleware;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
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
                cfg.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = AuthOptions.JwtIssuer,
                    ValidAudience = AuthOptions.JwtIssuer,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AuthOptions.JwtKey)),
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "role"
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
                Name = "Authorization",
                In = ParameterLocation.Header
            });
            c.OperationFilter<SecurityRequirementsOperationFilter>();
        });
        return collection;
    }
}
