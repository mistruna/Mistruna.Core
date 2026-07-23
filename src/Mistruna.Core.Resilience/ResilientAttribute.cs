namespace Mistruna.Core.Resilience;

/// <summary>Marks a MediatR request for optional retry via <see cref="Behaviors.ResilientCommandBehavior{TRequest,TResponse}"/>.</summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, Inherited = false)]
public sealed class ResilientAttribute : Attribute;
