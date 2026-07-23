using MediatR;

namespace Mistruna.Core.Abstractions.Cqrs;

/// <summary>Marks a command that does not return a value.</summary>
public interface ICommand : IRequest;

/// <summary>Marks a command that returns a response.</summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface ICommand<out TResponse> : IRequest<TResponse>;
