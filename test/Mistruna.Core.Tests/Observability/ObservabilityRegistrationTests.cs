using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Observability.DependencyInjection;
using Xunit;

namespace Mistruna.Core.Tests.Observability;

public sealed class ObservabilityRegistrationTests
{
    [Fact]
    public void AddMistrunaObservability_ShouldConfigureOtlpExporterWhenEndpointIsPresent()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OpenTelemetry:Otlp:Endpoint"] = "http://collector:4317"
            })
            .Build();
        var services = new ServiceCollection();
        var servicesWithoutEndpoint = new ServiceCollection();

        services.AddMistrunaObservability(configuration);
        servicesWithoutEndpoint.AddMistrunaObservability(new ConfigurationBuilder().Build());

        CountOtlpRegistrations(services).Should().BeGreaterThan(CountOtlpRegistrations(servicesWithoutEndpoint));
    }

    private static int CountOtlpRegistrations(IServiceCollection services) =>
        services.Count(descriptor =>
            descriptor.ToString().Contains("Otlp", StringComparison.OrdinalIgnoreCase));
}
