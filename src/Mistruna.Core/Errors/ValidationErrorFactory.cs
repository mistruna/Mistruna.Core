using FluentValidation.Results;

namespace Mistruna.Core.Errors;

public static class ValidationErrorFactory
{
    /// <summary>
    /// Creates a single validation error with the specified property name, error message, and optional error code.
    /// </summary>
    /// <param name="propertyName">The name of the property associated with the error.</param>
    /// <param name="errorMessage">The error message describing the validation failure.</param>
    /// <param name="errorCode">An optional error code for programmatic handling.</param>
    /// <returns>An enumerable containing a single validation failure.</returns>
    private static IEnumerable<ValidationFailure> CreateError(
        string propertyName,
        string errorMessage,
        string? errorCode = null) => new[] { new ValidationFailure(propertyName, errorMessage, errorCode) };

    /// <summary>
    /// Creates a validation error indicating that an entity was not found.
    /// </summary>
    /// <param name="entityType">The type of the entity (e.g., typeof(User)).</param>
    /// <returns>An enumerable containing a single validation failure.</returns>
    public static IEnumerable<ValidationFailure> CreateNotFoundError(Type entityType)
        => CreateError(entityType.Name,
            $"No {entityType.Name.ToLower()} was found.",
            $"{entityType.Name.ToUpper()}_NOT_FOUND");

    /// <summary>
    /// Creates a validation error indicating that an entity was not found by its ID.
    /// </summary>
    /// <param name="entityType">The type of the entity (e.g., typeof(User)).</param>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>An enumerable containing a single validation failure.</returns>
    public static IEnumerable<ValidationFailure> CreateNotFoundByIdError(Type entityType, Guid id)
        => CreateError($"{entityType.Name}.Id",
            $"No {entityType.Name.ToLower()} found with ID: {id}.",
            $"{entityType.Name.ToUpper()}_NOT_FOUND_BY_ID");
}
