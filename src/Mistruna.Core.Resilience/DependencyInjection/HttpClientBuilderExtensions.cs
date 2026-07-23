using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http.Resilience;

namespace Mistruna.Core.Resilience.DependencyInjection;

/// <summary>HttpClient builder extensions for Mistruna resilience presets.</summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Adds a Mistruna resilience handler to the HttpClient using the named preset
    /// (<see cref="MistrunaResiliencePresets.Standard"/> by default).
    /// </summary>
    public static IHttpClientBuilder AddMistrunaResilienceHandler(
        this IHttpClientBuilder builder,
        string preset = MistrunaResiliencePresets.Standard)
    {
        ArgumentNullException.ThrowIfNull(builder);

        if (string.Equals(preset, MistrunaResiliencePresets.Disable, StringComparison.OrdinalIgnoreCase))
            return builder;

        if (!MistrunaResiliencePresets.IsKnown(preset))
        {
            throw new ArgumentException(
                $"Unknown resilience preset '{preset}'. Expected one of: {MistrunaResiliencePresets.Standard}, {MistrunaResiliencePresets.Aggressive}, {MistrunaResiliencePresets.Disable}.",
                nameof(preset));
        }

        if (string.Equals(preset, MistrunaResiliencePresets.Aggressive, StringComparison.OrdinalIgnoreCase))
        {
            builder.AddStandardResilienceHandler(static options =>
            {
                options.Retry.MaxRetryAttempts = 5;
                options.Retry.Delay = TimeSpan.FromMilliseconds(200);
                options.AttemptTimeout.Timeout = TimeSpan.FromSeconds(5);
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(30);
            });

            return builder;
        }

        builder.AddStandardResilienceHandler();
        return builder;
    }
}
