using System.Diagnostics;
using FluentAssertions;
using MediatR;
using Mistruna.Core.Abstractions.Cqrs;
using Mistruna.Core.Abstractions.Results;
using Mistruna.Core.Observability;
using Mistruna.Core.Observability.Behaviors;
using Xunit;

namespace Mistruna.Core.Tests.Observability;

public sealed class OpenTelemetryMediatorBehaviorTests : IDisposable
{
    private sealed record SampleCommand : ICommand<string>;

    private sealed record SampleQuery : IQuery<int>;

    private readonly List<Activity> _activities = [];
    private readonly ActivityListener _listener;

    public OpenTelemetryMediatorBehaviorTests()
    {
        _listener = new ActivityListener
        {
            ShouldListenTo = source => source.Name == MistrunaMediatorTelemetry.ActivitySourceName,
            Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllData,
            ActivityStarted = activity => _activities.Add(activity),
        };

        ActivitySource.AddActivityListener(_listener);
    }

    [Fact]
    public async Task Handle_sets_request_type_and_request_kind_for_command()
    {
        var behavior = new OpenTelemetryMediatorBehavior<SampleCommand, string>();

        await behavior.Handle(
            new SampleCommand(),
            _ => Task.FromResult("ok"),
            CancellationToken.None);

        var activity = _activities.Should().ContainSingle().Subject;
        activity.GetTagItem("request.type").Should().Be(typeof(SampleCommand).FullName);
        activity.GetTagItem("request.kind").Should().Be("command");
        activity.GetTagItem("outcome").Should().Be("success");
    }

    [Fact]
    public async Task Handle_sets_request_type_and_request_kind_for_query()
    {
        var behavior = new OpenTelemetryMediatorBehavior<SampleQuery, int>();

        await behavior.Handle(
            new SampleQuery(),
            _ => Task.FromResult(42),
            CancellationToken.None);

        var activity = _activities.Should().ContainSingle().Subject;
        activity.GetTagItem("request.type").Should().Be(typeof(SampleQuery).FullName);
        activity.GetTagItem("request.kind").Should().Be("query");
        activity.GetTagItem("outcome").Should().Be("success");
    }

    [Fact]
    public async Task Handle_sets_outcome_failure_when_result_fails()
    {
        var behavior = new OpenTelemetryMediatorBehavior<SampleCommand, Result<string>>();

        await behavior.Handle(
            new SampleCommand(),
            _ => Task.FromResult(Result.Failure<string>(Error.Failure("Test.Failed", "failed"))),
            CancellationToken.None);

        var activity = _activities.Should().ContainSingle().Subject;
        activity.GetTagItem("outcome").Should().Be("failure");
        activity.Status.Should().Be(ActivityStatusCode.Error);
    }

    [Fact]
    public async Task Handle_sets_outcome_error_when_handler_throws()
    {
        var behavior = new OpenTelemetryMediatorBehavior<SampleCommand, string>();

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            behavior.Handle(
                new SampleCommand(),
                _ => throw new InvalidOperationException("boom"),
                CancellationToken.None));

        var activity = _activities.Should().ContainSingle().Subject;
        activity.GetTagItem("outcome").Should().Be("error");
        activity.Status.Should().Be(ActivityStatusCode.Error);
    }

    public void Dispose() => _listener.Dispose();
}
