using FluentValidation;
using MediatR;
using Mistruna.Core.Abstractions.Results;

namespace Mistruna.Core.Behaviors;

/// <summary>
/// Validates requests and returns a failed result when the response supports it.
/// </summary>
public sealed class ResultValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : class
{
    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next(cancellationToken);

        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context, cancellationToken)));
        var failures = validationResults
            .SelectMany(result => result.Errors)
            .Where(failure => failure is not null)
            .ToList();

        return failures.Count == 0
            ? await next(cancellationToken)
            : CreateValidationErrorResult(failures);
    }

    private static TResponse CreateValidationErrorResult(
        List<FluentValidation.Results.ValidationFailure> failures)
    {
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var error = Error.Validation(
                "Validation.Failed",
                string.Join(
                    "; ",
                    failures.Select(failure =>
                        $"{failure.PropertyName}: {failure.ErrorMessage}")));
            var resultType = typeof(Result<>).MakeGenericType(valueType);
            var failureMethod = resultType.GetMethod("Failure", [typeof(Error)]);

            return (TResponse)failureMethod!.Invoke(null, [error])!;
        }

        throw new ValidationException(failures);
    }
}
