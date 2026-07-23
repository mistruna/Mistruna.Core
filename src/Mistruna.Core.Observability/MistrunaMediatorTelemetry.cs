using System.Diagnostics;

namespace Mistruna.Core.Observability;

/// <summary>Shared OpenTelemetry identifiers for MediatR instrumentation.</summary>
public static class MistrunaMediatorTelemetry
{
    /// <summary>The ActivitySource name used for MediatR request tracing.</summary>
    public const string ActivitySourceName = "mistruna.mediator";

    internal static readonly ActivitySource ActivitySource = new(ActivitySourceName);
}
