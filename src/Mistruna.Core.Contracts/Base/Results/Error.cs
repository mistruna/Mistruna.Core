namespace Mistruna.Core.Contracts.Base.Results;

/// <summary>
/// Represents an error with a code and description.
/// </summary>
/// <remarks>
/// Use this record to create strongly-typed errors that can be used with the Result pattern.
/// </remarks>
/// <example>
/// <code>
/// public static class UserErrors
/// {
///     public static readonly Error NotFound = new("User.NotFound", "User was not found");
///     public static readonly Error EmailInUse = new("User.EmailInUse", "Email is already in use");
/// }
/// </code>
/// </example>
public sealed record Error
{
    /// <summary>
    /// Represents no error (success state).
    /// </summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>
    /// Represents a null value error.
    /// </summary>
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided", ErrorType.Validation);

    /// <summary>
    /// Initializes a new instance of the <see cref="Error"/> record.
    /// </summary>
    /// <param name="code">The unique error code.</param>
    /// <param name="description">A human-readable description of the error.</param>
    /// <param name="type">The type of error.</param>
    public Error(string code, string description, ErrorType type = ErrorType.Failure)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>
    /// Gets the unique error code.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the human-readable description of the error.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Gets the type of error.
    /// </summary>
    public ErrorType Type { get; }

    /// <summary>
    /// Creates a failure error.
    /// </summary>
    public static Error Failure(string code, string description) =>
        new(code, description, ErrorType.Failure);

    /// <summary>
    /// Creates a validation error.
    /// </summary>
    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);

    /// <summary>
    /// Creates a not found error.
    /// </summary>
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    /// <summary>
    /// Creates a conflict error.
    /// </summary>
    public static Error Conflict(string code, string description) =>
        new(code, description, ErrorType.Conflict);

    /// <summary>
    /// Creates an unauthorized error.
    /// </summary>
    public static Error Unauthorized(string code, string description) =>
        new(code, description, ErrorType.Unauthorized);

    /// <summary>
    /// Creates a forbidden error.
    /// </summary>
    public static Error Forbidden(string code, string description) =>
        new(code, description, ErrorType.Forbidden);

    /// <summary>
    /// Implicitly converts an error to a string (returns the code).
    /// </summary>
    public static implicit operator string(Error error) => error.Code;

    /// <summary>
    /// Returns the error code as string representation.
    /// </summary>
    public override string ToString() => Code;
}

/// <summary>
/// Represents the type of error.
/// </summary>
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
