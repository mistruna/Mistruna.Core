using FluentValidation.Results;

namespace Mistruna.Core.Errors;

public static class UserErrors
{
    public static IEnumerable<ValidationFailure> NotFound() => new List<ValidationFailure>
        { new(nameof(Nullable), "User not found.") };

    public static IEnumerable<ValidationFailure> NotFound(Guid id) => new List<ValidationFailure>
        { new(nameof(id), $"User not found. Id: {id}") };

    public static IEnumerable<ValidationFailure> NotFoundByUsername(string username) => new List<ValidationFailure>
        { new(nameof(username), $"User not found by username. Username: {username}") };

    public static IEnumerable<ValidationFailure> NotFoundByEmail(string email) => new List<ValidationFailure>
        { new(nameof(email), $"User not found by email. Email: {email}") };

    public static IEnumerable<ValidationFailure> PasswordIsInvalid() => new List<ValidationFailure>
        { new(nameof(Nullable), "Password is invalid.") };

    public static IEnumerable<ValidationFailure> EmailIsAlreadyInUse() => new List<ValidationFailure>
        { new(nameof(Nullable), "Email is already in use.") };

    public static IEnumerable<ValidationFailure> UsernameIsAlreadyInUse() => new List<ValidationFailure>
        { new(nameof(Nullable), "Username is already in use.") };
}
