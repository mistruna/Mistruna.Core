using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace Mistruna.Core.AspNetCore.Cors;

public static class CorsServiceCollectionExtensions
{
    public const string DefaultPolicyName = "Mistruna";

    public static IServiceCollection AddMistrunaCors(
        this IServiceCollection services,
        Action<MistrunaCorsOptions>? configure = null,
        string policyName = DefaultPolicyName)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = new MistrunaCorsOptions();
        configure?.Invoke(options);

        if (options.AllowAnyOrigin && options.AllowCredentials)
        {
            throw new InvalidOperationException(
                "CORS cannot allow credentials together with any origin.");
        }

        services.AddCors(cors => cors.AddPolicy(policyName, policy =>
        {
            ConfigureOrigins(policy, options);
            ConfigureMethods(policy, options);
            ConfigureHeaders(policy, options);

            if (options.AllowCredentials)
            {
                policy.AllowCredentials();
            }
        }));

        return services;
    }

    private static void ConfigureOrigins(CorsPolicyBuilder policy, MistrunaCorsOptions options)
    {
        if (options.AllowAnyOrigin || options.AllowedOrigins.Count == 0)
        {
            policy.AllowAnyOrigin();
            return;
        }

        policy.WithOrigins(options.AllowedOrigins.ToArray());
    }

    private static void ConfigureMethods(CorsPolicyBuilder policy, MistrunaCorsOptions options)
    {
        if (options.AllowAnyMethod || options.AllowedMethods.Count == 0)
        {
            policy.AllowAnyMethod();
            return;
        }

        policy.WithMethods(options.AllowedMethods.ToArray());
    }

    private static void ConfigureHeaders(CorsPolicyBuilder policy, MistrunaCorsOptions options)
    {
        if (options.AllowAnyHeader || options.AllowedHeaders.Count == 0)
        {
            policy.AllowAnyHeader();
            return;
        }

        policy.WithHeaders(options.AllowedHeaders.ToArray());
    }
}
