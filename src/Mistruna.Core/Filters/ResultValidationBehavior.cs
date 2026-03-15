using FluentValidation;
using MediatR;
using Mistruna.Core.Contracts.Base.Results;

namespace Mistruna.Core.Filters;

/// <summary>
/// A MediatR pipeline behavior that validates requests using FluentValidation
/// and returns a Result with validation errors instead of throwing exceptions.
/// </summary>
/// <typeparam name="TRequest">The request type.</typeparam>
/// <typeparam name="TResponse">The response type (must be a Result).</typeparam>
public class ResultValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ResultValidationBehavior{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">The validators for the request.</param>
    public ResultValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        if (failures.Count != 0)
        {
            return CreateValidationErrorResult(failures);
        }

        return await next(cancellationToken);
    }

    private static TResponse CreateValidationErrorResult(List<FluentValidation.Results.ValidationFailure> failures)
    {
        var responseType = typeof(TResponse);

        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var valueType = responseType.GetGenericArguments()[0];
            var error = Error.Validation(
                "Validation.Failed",
                string.Join("; ", failures.Select(f => $"{f.PropertyName}: {f.ErrorMessage}")));

            var resultType = typeof(Result<>).MakeGenericType(valueType);
            var failureMethod = resultType.GetMethod("Failure", new[] { typeof(Error) });

            return (TResponse)failureMethod!.Invoke(null, new object[] { error })!;
        }

        throw new ValidationException(failures);
    }
}
