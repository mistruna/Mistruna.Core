namespace Mistruna.Core.Base.Responses;

/// <summary>
/// Represents a standardized error response.
/// </summary>
public class ErrorResponse
{
    /// <summary>Gets or sets the error message.</summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>Gets or sets the error code.</summary>
    public string? ErrorCode { get; set; }

    /// <summary>Gets or sets additional error details.</summary>
    public object? Details { get; set; }

    /// <summary>Gets or sets the trace identifier for debugging.</summary>
    public string? TraceId { get; set; }

    /// <summary>Gets or sets the timestamp when the error occurred.</summary>
    public DateTime Timestamp { get; set; }
}
