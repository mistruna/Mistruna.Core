using FluentValidation.Results;

namespace Mistruna.Core.Exceptions;

/// <summary>
/// Exception thrown when a requested resource is not found.
/// This exception is typically handled by middleware to return HTTP 404 status code.
/// </summary>
/// <remarks>
/// Use this exception when an entity or resource cannot be found in the database or storage.
/// The <see cref="ExceptionHandlingMiddleware"/> will automatically convert this to an HTTP 404 response.
/// </remarks>
/// <example>
/// <code>
/// var entity = await repository.GetByIdAsync(id)
///     ?? throw new NotFoundException($"Entity with id {id} not found");
/// </code>
/// </example>
public class NotFoundException : CoreException
{
    private const string DefaultErrorCode = "NOT_FOUND";

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class.
    /// </summary>
    public NotFoundException()
        : base(DefaultErrorCode, "The requested resource was not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NotFoundException(string message)
        : base(DefaultErrorCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a custom error code.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="errorCode">The custom error code.</param>
    public NotFoundException(string message, string errorCode)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class for a specific entity type.
    /// </summary>
    /// <param name="entityType">The type of entity that was not found.</param>
    /// <param name="id">The ID of the entity.</param>
    public NotFoundException(Type entityType, object id)
        : base($"{entityType.Name.ToUpperInvariant()}_NOT_FOUND",
              $"{entityType.Name} with identifier '{id}' was not found.")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with validation errors.
    /// </summary>
    /// <param name="errors">A collection of validation failures to include in the error message.</param>
    public NotFoundException(IEnumerable<ValidationFailure> errors)
        : base(DefaultErrorCode, string.Join(Environment.NewLine, errors.Select(e => e.ErrorMessage)))
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NotFoundException(string message, Exception innerException)
        : base(DefaultErrorCode, message, innerException: innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NotFoundException"/> class with multiple error messages.
    /// </summary>
    /// <param name="messages">An array of error messages to combine.</param>
    public NotFoundException(params string[] messages)
        : base(DefaultErrorCode, string.Join(Environment.NewLine, messages))
    {
    }

    /// <summary>
    /// Creates a NotFoundException for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="id">The identifier of the entity.</param>
    public static NotFoundException For<T>(object id) => new(typeof(T), id);

    /// <summary>
    /// Creates a NotFoundException for a specific entity type.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    public static NotFoundException For<T>() =>
        new($"{typeof(T).Name.ToUpperInvariant()}_NOT_FOUND", $"{typeof(T).Name} was not found.");
}
