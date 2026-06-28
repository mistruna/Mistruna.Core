using MediatR;

namespace Mistruna.Core.Abstractions;

/// <summary>
/// Marker interface for queries (read-only operations).
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
/// <remarks>
/// Queries should not change application state. Register audit or metering behaviors
/// only for <see cref="ICommand"/> / <see cref="ICommand{TResponse}"/> requests.
/// </remarks>
public interface IQuery<out TResponse> : IRequest<TResponse>;
