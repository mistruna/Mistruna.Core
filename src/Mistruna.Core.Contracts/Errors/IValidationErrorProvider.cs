using FluentValidation.Results;

namespace Mistruna.Core.Contracts.Errors;

/// <summary>
/// Provides methods to create validation error messages for common scenarios.
/// </summary>
public interface IValidationErrorProvider
{
    /// <summary>
    /// Returns a validation error indicating that an entity was not found.
    /// </summary>
    IEnumerable<ValidationFailure> NotFound();

    /// <summary>
    /// Returns a validation error indicating that an entity was not found by its Id.
    /// </summary>
    /// <param name="id">The Id of the entity.</param>
    IEnumerable<ValidationFailure> NotFound(Guid id);
}
