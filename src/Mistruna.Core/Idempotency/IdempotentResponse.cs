namespace Mistruna.Core.Idempotency;

/// <summary>
/// Serialized snapshot of an HTTP response used for idempotent replay.
/// </summary>
/// <param name="StatusCode">The HTTP status code of the original response.</param>
/// <param name="ContentType">Response Content-Type (may be null for 204).</param>
/// <param name="Body">The response body bytes (may be empty).</param>
public sealed record IdempotentResponse(int StatusCode, string? ContentType, byte[] Body);
