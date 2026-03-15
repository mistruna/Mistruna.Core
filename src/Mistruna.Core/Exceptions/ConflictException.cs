namespace Mistruna.Core.Exceptions;

/// <summary>
/// Exception thrown when a conflict occurs during resource modification.
/// This exception is typically handled by middleware to return HTTP 409 status code.
/// </summary>
/// <remarks>
/// Use this exception when there is a conflict with the current state of the resource,
/// such as duplicate entries, concurrent modification conflicts, or business rule violations.
/// </remarks>
/// <example>
/// <code>
/// if (await repository.ExistsAsync(email))
///     throw new ConflictException($"User with email {email} already exists");
/// </code>
/// </example>
public class ConflictException : CoreException
{
    private const string DefaultErrorCode = "CONFLICT";

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ConflictException(string message)
        : base(DefaultErrorCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class with a custom error code.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The custom error code.</param>
    public ConflictException(string message, string errorCode)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConflictException"/> class for duplicate entity.
    /// </summary>
    /// <param name="entityType">The type of entity that caused the conflict.</param>
    /// <param name="identifier">The identifier that caused the conflict.</param>
    public ConflictException(Type entityType, object identifier)
        : base($"{entityType.Name.ToUpperInvariant()}_ALREADY_EXISTS",
              $"{entityType.Name} with identifier '{identifier}' already exists.")
    {
    }

    /// <summary>
    /// Creates a ConflictException for a duplicate entity.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="identifier">The identifier that caused the conflict.</param>
    public static ConflictException For<T>(object identifier) => new(typeof(T), identifier);

    /// <summary>
    /// Creates a ConflictException for a duplicate entity with a custom field.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="fieldName">The field name that has a duplicate value.</param>
    /// <param name="fieldValue">The duplicate value.</param>
    public static ConflictException ForField<T>(string fieldName, object fieldValue) =>
        new($"{typeof(T).Name} with {fieldName} '{fieldValue}' already exists.",
            $"{typeof(T).Name.ToUpperInvariant()}_DUPLICATE_{fieldName.ToUpperInvariant()}");
}
