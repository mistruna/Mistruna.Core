using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Resilience.Behaviors;
using Mistruna.Core.Resilience.Internal;
using Polly;
using Polly.Retry;

namespace Mistruna.Core.Resilience.DependencyInjection;

/// <summary>Registers Mistruna resilience presets and optional marked-command MediatR retry.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers named HttpClient resilience presets (<see cref="MistrunaResiliencePresets.Standard"/>,
    /// <see cref="MistrunaResiliencePresets.Aggressive"/>, <see cref="MistrunaResiliencePresets.Disable"/>)
    /// and <see cref="ResilientCommandBehavior{TRequest,TResponse}"/> for MediatR.
    /// </summary>
    public static IServiceCollection AddMistrunaResilience(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddResiliencePipelines<string>(context =>
        {
            context.AddResiliencePipeline<HttpResponseMessage>(
                MistrunaResiliencePresets.Standard,
                static (builder, _) => MistrunaHttpResiliencePipelineConfigurator.ConfigureStandard(builder));

            context.AddResiliencePipeline<HttpResponseMessage>(
                MistrunaResiliencePresets.Aggressive,
                static (builder, _) => MistrunaHttpResiliencePipelineConfigurator.ConfigureAggressive(builder));

            context.AddResiliencePipeline<HttpResponseMessage>(
                MistrunaResiliencePresets.Disable,
                static (_, _) => { });

            context.AddResiliencePipeline(
                MistrunaResiliencePresets.MediatorCommand,
                static (builder, _) =>
                {
                    builder.AddRetry(new RetryStrategyOptions
                    {
                        MaxRetryAttempts = 3,
                        BackoffType = DelayBackoffType.Exponential,
                        UseJitter = true,
                    });
                });
        });

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ResilientCommandBehavior<,>));

        return services;
    }
}
