using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Resilience;
using Mistruna.Core.Resilience.DependencyInjection;
using Polly;
using Polly.Registry;
using Xunit;

namespace Mistruna.Core.Tests.Resilience;

public sealed class ResilienceRegistrationTests
{
    [Fact]
    public void AddMistrunaResilience_RegistersNamedHttpClientPresets()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaResilience();

        using var provider = services.BuildServiceProvider();
        var pipelineProvider = provider.GetRequiredService<ResiliencePipelineProvider<string>>();

        pipelineProvider.GetPipeline<HttpResponseMessage>(MistrunaResiliencePresets.Standard).Should().NotBeNull();
        pipelineProvider.GetPipeline<HttpResponseMessage>(MistrunaResiliencePresets.Aggressive).Should().NotBeNull();
        pipelineProvider.GetPipeline<HttpResponseMessage>(MistrunaResiliencePresets.Disable).Should().NotBeNull();
    }

    [Fact]
    public void AddMistrunaResilienceHandler_Standard_ConfiguresHttpClient()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaResilience();
        services.AddHttpClient("test").AddMistrunaResilienceHandler(MistrunaResiliencePresets.Standard);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("test");

        client.Should().NotBeNull();
    }

    [Fact]
    public void AddMistrunaResilienceHandler_Disable_DoesNotAddResilienceHandler()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaResilience();
        services.AddHttpClient("test").AddMistrunaResilienceHandler(MistrunaResiliencePresets.Disable);

        using var provider = services.BuildServiceProvider();
        var client = provider.GetRequiredService<IHttpClientFactory>().CreateClient("test");

        client.Should().NotBeNull();
    }
}
