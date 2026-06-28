namespace Mistruna.Core.Abstractions;

/// <summary>
/// Helpers for distinguishing CQRS request types in MediatR pipeline behaviors.
/// </summary>
public static class RequestKind
{
    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="requestType"/> implements
    /// <see cref="ICommand"/> or <see cref="ICommand{TResponse}"/>.
    /// </summary>
    public static bool IsCommand(Type requestType)
    {
        ArgumentNullException.ThrowIfNull(requestType);

        return typeof(ICommand).IsAssignableFrom(requestType) ||
               requestType.GetInterfaces().Any(IsCommandInterface);
    }

    /// <summary>
    /// Returns <see langword="true"/> when <paramref name="requestType"/> implements
    /// <see cref="IQuery{TResponse}"/>.
    /// </summary>
    public static bool IsQuery(Type requestType)
    {
        ArgumentNullException.ThrowIfNull(requestType);

        return requestType.GetInterfaces().Any(IsQueryInterface);
    }

    private static bool IsCommandInterface(Type interfaceType) =>
        interfaceType.IsGenericType &&
        interfaceType.GetGenericTypeDefinition() == typeof(ICommand<>);

    private static bool IsQueryInterface(Type interfaceType) =>
        interfaceType.IsGenericType &&
        interfaceType.GetGenericTypeDefinition() == typeof(IQuery<>);
}
