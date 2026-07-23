namespace Mistruna.Core.Abstractions.Results;

/// <summary>Identifies the category of an error.</summary>
public enum ErrorType
{
    /// <summary>No error.</summary>
    None = 0,
    /// <summary>General failure.</summary>
    Failure = 1,
    /// <summary>Validation error.</summary>
    Validation = 2,
    /// <summary>Resource not found.</summary>
    NotFound = 3,
    /// <summary>Resource conflict.</summary>
    Conflict = 4,
    /// <summary>Unauthorized access.</summary>
    Unauthorized = 5,
    /// <summary>Forbidden access.</summary>
    Forbidden = 6
}
