using FluentValidation.Results;

namespace Mistruna.Core.Abstractions.Errors;

/// <summary>Creates common validation failures.</summary>
public interface IValidationErrorProvider
{
    /// <summary>Returns a not-found validation failure.</summary>
    IEnumerable<ValidationFailure> NotFound();
    /// <summary>Returns a not-found validation failure for an identifier.</summary>
    IEnumerable<ValidationFailure> NotFound(Guid id);
}
