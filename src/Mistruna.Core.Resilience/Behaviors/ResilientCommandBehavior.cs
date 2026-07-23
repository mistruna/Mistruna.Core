using System.Reflection;
using MediatR;
using Polly;
using Polly.Registry;

namespace Mistruna.Core.Resilience.Behaviors;

/// <summary>Retries MediatR requests only when marked with <see cref="ResilientAttribute"/>.</summary>
public sealed class ResilientCommandBehavior<TRequest, TResponse>(
    ResiliencePipelineProvider<string> pipelineProvider) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    /// <inheritdoc />
    public Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!IsResilient(typeof(TRequest)))
            return next(cancellationToken);

        var pipeline = pipelineProvider.GetPipeline(MistrunaResiliencePresets.MediatorCommand);
        return pipeline.ExecuteAsync(
            static async (state, cancellationToken) => await state(cancellationToken),
            next,
            cancellationToken).AsTask();
    }

    private static bool IsResilient(Type requestType) =>
        requestType.GetCustomAttribute<ResilientAttribute>() is not null;
}
