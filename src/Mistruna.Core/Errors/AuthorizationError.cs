using FluentValidation.Results;

namespace Mistruna.Core.Errors;

public static class AuthorizationError
{
    public static IEnumerable<ValidationFailure> UnableToGetAuthorizationToken() => new List<ValidationFailure>
        { new("AuthorizationToken", "Unable to get authorization token") };
}
