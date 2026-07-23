namespace Mistruna.Core.Abstractions.Cqrs;

/// <summary>Classifies CQRS request types.</summary>
public static class RequestKind
{
    /// <summary>Returns whether a type implements a command marker.</summary>
    public static bool IsCommand(Type requestType)
    {
        ArgumentNullException.ThrowIfNull(requestType);
        return typeof(ICommand).IsAssignableFrom(requestType) ||
               requestType.GetInterfaces().Any(IsCommandInterface);
    }

    /// <summary>Returns whether a type implements a query marker.</summary>
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
