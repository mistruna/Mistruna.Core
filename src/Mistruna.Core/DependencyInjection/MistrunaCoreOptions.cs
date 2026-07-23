using System.Reflection;

namespace Mistruna.Core.DependencyInjection;

/// <summary>Configures the Mistruna.Core service registrations.</summary>
public sealed class MistrunaCoreOptions
{
    internal List<Assembly> Assemblies { get; } = [];

    internal bool EnableValidation { get; private set; }

    internal bool EnableLoggingBehavior { get; private set; }

    /// <summary>Adds assemblies containing MediatR handlers and validators.</summary>
    public MistrunaCoreOptions RegisterAssemblies(params Assembly[] assemblies)
    {
        ArgumentNullException.ThrowIfNull(assemblies);
        Assemblies.AddRange(assemblies);
        return this;
    }

    /// <summary>Enables FluentValidation validator discovery and request validation.</summary>
    public MistrunaCoreOptions AddValidation()
    {
        EnableValidation = true;
        return this;
    }

    /// <summary>Enables request execution logging.</summary>
    public MistrunaCoreOptions AddLoggingBehavior()
    {
        EnableLoggingBehavior = true;
        return this;
    }
}
