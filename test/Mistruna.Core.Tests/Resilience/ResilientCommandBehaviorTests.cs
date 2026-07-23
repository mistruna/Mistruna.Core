using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Mistruna.Core.Abstractions.Cqrs;
using Mistruna.Core.Resilience;
using Mistruna.Core.Resilience.Behaviors;
using Mistruna.Core.Resilience.DependencyInjection;
using Polly.Registry;
using Xunit;

namespace Mistruna.Core.Tests.Resilience;

public sealed class ResilientCommandBehaviorTests : IDisposable
{
    private sealed record UnmarkedCommand : ICommand<string>;

    [Resilient]
    private sealed record MarkedCommand : ICommand<string>;

    private readonly ServiceProvider _provider;

    public ResilientCommandBehaviorTests()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddMistrunaResilience();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Handle_does_not_retry_unmarked_command()
    {
        var attempts = 0;
        var behavior = CreateBehavior<UnmarkedCommand, string>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            behavior.Handle(
                new UnmarkedCommand(),
                _ =>
                {
                    attempts++;
                    throw new InvalidOperationException("boom");
                },
                CancellationToken.None));

        attempts.Should().Be(1);
    }

    [Fact]
    public async Task Handle_retries_marked_command_on_transient_failure()
    {
        var attempts = 0;
        var behavior = CreateBehavior<MarkedCommand, string>();

        var result = await behavior.Handle(
            new MarkedCommand(),
            _ =>
            {
                attempts++;
                if (attempts < 3)
                    throw new InvalidOperationException("transient");

                return Task.FromResult("ok");
            },
            CancellationToken.None);

        result.Should().Be("ok");
        attempts.Should().Be(3);
    }

    private ResilientCommandBehavior<TRequest, TResponse> CreateBehavior<TRequest, TResponse>()
        where TRequest : notnull
        => new(_provider.GetRequiredService<ResiliencePipelineProvider<string>>());

    public void Dispose() => _provider.Dispose();
}
