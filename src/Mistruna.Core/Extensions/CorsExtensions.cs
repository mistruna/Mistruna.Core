using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Microservices.Core.Configurations;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Extension methods for configuring CORS in the DI container.
/// </summary>
public static class CorsExtensions
{
    /// <summary>
    /// Adds CORS policy configuration to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="corsPolicy">The CORS policy options.</param>
    /// <param name="policyName">The name of the CORS policy. Defaults to "AllowAll".</param>
    /// <returns>The service collection.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="corsPolicy"/> is null.</exception>
    public static IServiceCollection AddCorsPolicy(
        this IServiceCollection services,
        CorsPolicyOptions corsPolicy,
        string policyName = "AllowAll")
    {
        ArgumentNullException.ThrowIfNull(corsPolicy);

        services.AddCors(options => options.AddPolicy(policyName, corsPolicyBuilder =>
        {
            ConfigureOrigins(corsPolicyBuilder, corsPolicy.AllowAnyOrigin, corsPolicy.AllowedOrigins);
            ConfigureMethods(corsPolicyBuilder, corsPolicy.AllowAnyMethod, corsPolicy.AllowedMethods);
            ConfigureHeaders(corsPolicyBuilder, corsPolicy.AllowAnyHeader, corsPolicy.AllowedHeaders);

            if (corsPolicy.AllowCredentials)
            {
                corsPolicyBuilder.AllowCredentials();
            }

        }));

        return services;
    }

    private static void ConfigureOrigins(CorsPolicyBuilder builder, bool allowAny, List<string>? allowed)
    {
        if (allowAny || allowed is not { Count: > 0 })
        {
            builder.AllowAnyOrigin();
        }
        else
        {
            builder.WithOrigins(allowed.ToArray());
        }
    }

    private static void ConfigureMethods(CorsPolicyBuilder builder, bool allowAny, List<string>? allowed)
    {
        if (allowAny || allowed is not { Count: > 0 })
        {
            builder.AllowAnyMethod();
        }
        else
        {
            builder.WithMethods(allowed.ToArray());
        }
    }

    private static void ConfigureHeaders(CorsPolicyBuilder builder, bool allowAny, List<string>? allowed)
    {
        if (allowAny || allowed is not { Count: > 0 })
        {
            builder.AllowAnyHeader();
        }
        else
        {
            builder.WithHeaders(allowed.ToArray());
        }
    }
}
