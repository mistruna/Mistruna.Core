using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Observability.Behaviors;
using OpenTelemetry;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Mistruna.Core.Observability.DependencyInjection;

/// <summary>Registers Mistruna OpenTelemetry tracing, metrics, and MediatR instrumentation.</summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenTelemetry tracing and metrics with ASP.NET Core and HTTP client instrumentation,
    /// registers <see cref="OpenTelemetryMediatorBehavior{TRequest,TResponse}"/> for MediatR,
    /// and enables OTLP export when <c>OpenTelemetry:Otlp:Endpoint</c> is configured.
    /// </summary>
    public static IServiceCollection AddMistrunaObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var openTelemetry = services.AddOpenTelemetry()
            .ConfigureResource(resource =>
            {
                var serviceName = configuration["OpenTelemetry:ServiceName"]
                    ?? configuration["Mistruna:Observability:ServiceName"];

                if (!string.IsNullOrWhiteSpace(serviceName))
                {
                    resource.AddService(serviceName);
                }
            })
            .WithTracing(tracing => tracing
                .AddSource(MistrunaMediatorTelemetry.ActivitySourceName)
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation());

        var otlpEndpoint = configuration["OpenTelemetry:Otlp:Endpoint"];
        if (!string.IsNullOrWhiteSpace(otlpEndpoint))
        {
            openTelemetry.UseOtlpExporter(
                OtlpExportProtocol.Grpc,
                new Uri(otlpEndpoint, UriKind.Absolute));
        }

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(OpenTelemetryMediatorBehavior<,>));

        return services;
    }
}
