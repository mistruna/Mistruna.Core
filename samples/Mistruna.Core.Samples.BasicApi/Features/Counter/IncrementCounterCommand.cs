using MediatR;
using Mistruna.Core.Abstractions;

namespace Mistruna.Core.Samples.BasicApi.Features.Counter;

/// <summary>
/// Sample write request.
/// </summary>
public sealed class IncrementCounterCommand : ICommand<int>
{
    public int Step { get; init; } = 1;
}

internal sealed class IncrementCounterHandler : IRequestHandler<IncrementCounterCommand, int>
{
    private static int _counter;

    public Task<int> Handle(IncrementCounterCommand request, CancellationToken cancellationToken)
    {
        _counter += request.Step;
        return Task.FromResult(_counter);
    }
}
