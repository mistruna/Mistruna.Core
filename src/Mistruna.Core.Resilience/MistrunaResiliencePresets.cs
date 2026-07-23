namespace Mistruna.Core.Resilience;

/// <summary>Named HttpClient resilience preset identifiers.</summary>
public static class MistrunaResiliencePresets
{
    /// <summary>Default balanced retry, timeout, and circuit breaker settings.</summary>
    public const string Standard = "Standard";

    /// <summary>More retry attempts with shorter per-attempt timeouts.</summary>
    public const string Aggressive = "Aggressive";

    /// <summary>Disables HttpClient resilience handlers.</summary>
    public const string Disable = "Disable";

    /// <summary>MediatR pipeline key for marked command retry.</summary>
    internal const string MediatorCommand = "MediatorCommand";

    internal static bool IsKnown(string preset) =>
        string.Equals(preset, Standard, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(preset, Aggressive, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(preset, Disable, StringComparison.OrdinalIgnoreCase);
}
