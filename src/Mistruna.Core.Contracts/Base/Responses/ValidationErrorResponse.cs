namespace Mistruna.Core.Contracts.Base.Responses;

/// <summary>
/// Represents a validation error response returned in case of a 400 Bad Request status with validation errors.
/// </summary>
public class ValidationErrorResponse
{
    /// <summary>Gets or sets the type URI identifying the problem.</summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>Gets or sets the short, human-readable summary of the problem.</summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>Gets or sets the HTTP status code.</summary>
    public int Status { get; set; }

    /// <summary>Gets or sets the trace identifier for debugging.</summary>
    public string TraceId { get; set; } = string.Empty;

    /// <summary>Gets or sets the validation errors keyed by field name.</summary>
    public Dictionary<string, string[]> Errors { get; set; } = new();
}
