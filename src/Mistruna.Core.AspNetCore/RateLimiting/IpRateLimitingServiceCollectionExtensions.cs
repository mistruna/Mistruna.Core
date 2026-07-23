using Microsoft.Extensions.DependencyInjection;

namespace Mistruna.Core.AspNetCore.RateLimiting;

public static class IpRateLimitingServiceCollectionExtensions
{
    public static IServiceCollection AddMistrunaIpRateLimiting(
        this IServiceCollection services,
        Action<MistrunaIpRateLimitOptions>? configure = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        var options = services.AddOptions<MistrunaIpRateLimitOptions>();
        if (configure is not null)
        {
            options.Configure(configure);
        }

        options
            .Validate(value => value.RequestsPerWindow > 0, "RequestsPerWindow must be positive.")
            .Validate(value => value.WindowSeconds > 0, "WindowSeconds must be positive.")
            .ValidateOnStart();

        return services;
    }
}
