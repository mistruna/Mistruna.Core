using MediatR;

namespace Mistruna.Core.Abstractions;

/// <summary>
/// Marker interface for commands (write operations).
/// </summary>
public interface ICommand : IRequest;

/// <summary>
/// Marker interface for commands that return a response.
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>;
