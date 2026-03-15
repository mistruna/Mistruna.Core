namespace Mistruna.Core.Exceptions;

/// <summary>
/// Exception thrown when the request is invalid or contains bad data.
/// This exception is typically handled by middleware to return HTTP 400 status code.
/// </summary>
/// <example>
/// <code>
/// if (string.IsNullOrEmpty(request.Email))
///     throw new BadRequestException("Email is required", "VALIDATION_ERROR");
/// </code>
/// </example>
public class BadRequestException : CoreException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BadRequestException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code. Defaults to "BAD_REQUEST".</param>
    /// <param name="details">Additional error details.</param>
    public BadRequestException(
        string message,
        string errorCode = "BAD_REQUEST",
        object? details = null)
        : base(errorCode, message, details)
    {
    }
}
