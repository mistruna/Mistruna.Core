namespace Mistruna.Core.Idempotency;

public sealed class IdempotencyOptions
{
    /// <summary>Header name carrying the client-supplied key. Defaults to <c>Idempotency-Key</c>.</summary>
    public string HeaderName { get; set; } = "Idempotency-Key";

    /// <summary>Time-to-live for cached responses. Defaults to 24 hours.</summary>
    public TimeSpan Ttl { get; set; } = TimeSpan.FromHours(24);

    /// <summary>HTTP methods that participate in idempotency. Defaults to mutating verbs.</summary>
    public HashSet<string> Methods { get; } = new(StringComparer.OrdinalIgnoreCase) { "POST", "PUT", "PATCH", "DELETE" };

    /// <summary>Redis key prefix. Defaults to <c>idemp</c>.</summary>
    public string KeyPrefix { get; set; } = "idemp";
}
