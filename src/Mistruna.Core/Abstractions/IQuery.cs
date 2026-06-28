using MediatR;

namespace Mistruna.Core.Abstractions;

/// <summary>
/// Marker interface for queries (read-only operations).
/// </summary>
/// <typeparam name="TResponse">The response type.</typeparam>
public interface IQuery<out TResponse> : IRequest<TResponse>;
