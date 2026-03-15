using FluentValidation;
using MediatR;

namespace Mistruna.Core.Filters;

/// <summary>
/// MediatR pipeline behavior that validates requests using FluentValidation validators.
/// </summary>
/// <typeparam name="TRequest">The type of request being validated.</typeparam>
/// <typeparam name="TResponse">The type of response returned by the handler.</typeparam>
/// <remarks>
/// This behavior runs all registered validators for a request type before the request handler is executed.
/// If any validation failures occur, a <see cref="ValidationException"/> is thrown.
///
/// Register this behavior in your DI container:
/// <code>
/// services.AddTransient(typeof(IPipelineBehavior&lt;,&gt;), typeof(RequestValidationBehavior&lt;,&gt;));
/// </code>
/// </remarks>
/// <example>
/// <code>
/// // Create a validator for your request
/// public class CreateUserCommandValidator : AbstractValidator&lt;CreateUserCommand&gt;
/// {
///     public CreateUserCommandValidator()
///     {
///         RuleFor(x => x.Email).NotEmpty().EmailAddress();
///         RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
///     }
/// }
/// </code>
/// </example>
public class RequestValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : class
{
    /// <summary>
    /// Handles the validation of the request before passing it to the next handler in the pipeline.
    /// </summary>
    /// <param name="request">The request to validate.</param>
    /// <param name="next">The delegate for the next action in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the next handler in the pipeline.</returns>
    /// <exception cref="ValidationException">Thrown when validation fails.</exception>
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (!validators.Any())
        {
            return await next(cancellationToken);
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v =>
                v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .Where(r => r.Errors.Count > 0)
            .SelectMany(r => r.Errors)
            .ToList();

        if (failures.Count > 0)
        {
            throw new ValidationException(failures);
        }

        return await next(cancellationToken);
    }
}
