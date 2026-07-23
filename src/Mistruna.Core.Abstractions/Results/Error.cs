namespace Mistruna.Core.Abstractions.Results;

/// <summary>Represents an error with a code, description, and category.</summary>
public sealed record Error
{
    /// <summary>Represents no error.</summary>
    public static readonly Error None = new(string.Empty, string.Empty, ErrorType.None);

    /// <summary>Represents an unexpected null value.</summary>
    public static readonly Error NullValue =
        new("Error.NullValue", "A null value was provided", ErrorType.Validation);

    /// <summary>Initializes an error.</summary>
    public Error(string code, string description, ErrorType type = ErrorType.Failure)
    {
        Code = code;
        Description = description;
        Type = type;
    }

    /// <summary>Gets the unique error code.</summary>
    public string Code { get; }

    /// <summary>Gets the human-readable description.</summary>
    public string Description { get; }

    /// <summary>Gets the error category.</summary>
    public ErrorType Type { get; }

    /// <summary>Creates a general failure.</summary>
    public static Error Failure(string code, string description) => new(code, description, ErrorType.Failure);

    /// <summary>Creates a validation error.</summary>
    public static Error Validation(string code, string description) => new(code, description, ErrorType.Validation);

    /// <summary>Creates a not-found error.</summary>
    public static Error NotFound(string code, string description) => new(code, description, ErrorType.NotFound);

    /// <summary>Creates a conflict error.</summary>
    public static Error Conflict(string code, string description) => new(code, description, ErrorType.Conflict);

    /// <summary>Creates an unauthorized error.</summary>
    public static Error Unauthorized(string code, string description) => new(code, description, ErrorType.Unauthorized);

    /// <summary>Creates a forbidden error.</summary>
    public static Error Forbidden(string code, string description) => new(code, description, ErrorType.Forbidden);

    /// <summary>Returns the error code.</summary>
    public static implicit operator string(Error error) => error.Code;

    /// <inheritdoc />
    public override string ToString() => Code;
}
