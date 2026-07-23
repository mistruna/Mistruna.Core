using MediatR;
using Mistruna.Core.Abstractions.Cqrs;

namespace Mistruna.Core.Samples.BasicApi.Features.Ping;

/// <summary>
/// Sample read-only request.
/// </summary>
public sealed record PingQuery(string Name) : IQuery<string>;

internal sealed class PingQueryHandler : IRequestHandler<PingQuery, string>
{
    public Task<string> Handle(PingQuery request, CancellationToken cancellationToken)
        => Task.FromResult($"Hello, {request.Name}!");
}
