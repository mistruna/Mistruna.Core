using MediatR;

namespace Mistruna.Core.Abstractions;

/// <summary>
/// Marker interface for commands (write operations).
/// Implement on MediatR requests that mutate application state.
/// </summary>
/// <remarks>
/// Use together with <see cref="IQuery{TResponse}"/> to separate reads from writes.
/// Pipeline behaviors can target commands only (for example audit logging).
/// </remarks>
public interface ICommand : IRequest;

/// <summary>
/// Marker interface for commands that return a response.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>;
