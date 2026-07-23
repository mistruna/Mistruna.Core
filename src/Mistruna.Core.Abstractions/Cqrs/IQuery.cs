using MediatR;

namespace Mistruna.Core.Abstractions.Cqrs;

/// <summary>Marks a read-only query that returns a response.</summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>;
