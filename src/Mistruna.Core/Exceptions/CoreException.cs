namespace Mistruna.Core.Exceptions;

/// <summary>
/// Base exception class for all application-specific exceptions.
/// Provides error code support for consistent error handling.
/// </summary>
public abstract class CoreException : Exception
{
    /// <summary>
    /// Gets the unique error code for this exception.
    /// </summary>
    public string ErrorCode { get; }

    /// <summary>
    /// Gets additional details about the error.
    /// </summary>
    public object? Details { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreException"/> class.
    /// </summary>
    /// <param name="errorCode">The unique error code.</param>
    /// <param name="message">The error message.</param>
    /// <param name="details">Additional error details.</param>
    /// <param name="innerException">The inner exception.</param>
    protected CoreException(
        string errorCode,
        string message,
        object? details = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
        Details = details;
    }
}
