namespace Mistruna.Core.Base.Responses;

/// <summary>
/// Represents a validation error response returned in case of a 400 Bad Request status with validation errors.
/// </summary>
public class ValidationErrorResponse
{
    public string Type { get; set; }

    public string Title { get; set; }

    public int Status { get; set; }

    public string TraceId { get; set; }

    public Dictionary<string, string[]> Errors { get; set; }
}
