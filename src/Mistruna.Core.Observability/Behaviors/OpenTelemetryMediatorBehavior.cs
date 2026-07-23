using System.Diagnostics;
using MediatR;
using Mistruna.Core.Abstractions.Cqrs;
using Mistruna.Core.Abstractions.Results;

namespace Mistruna.Core.Observability.Behaviors;

/// <summary>Creates OpenTelemetry activities for MediatR pipeline requests.</summary>
public sealed class OpenTelemetryMediatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{

    /// <inheritdoc />
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestType = typeof(TRequest);
        var requestKind = ResolveRequestKind(requestType);

        using var activity = MistrunaMediatorTelemetry.ActivitySource.StartActivity(requestType.Name);
        activity?.SetTag("request.type", requestType.FullName ?? requestType.Name);
        activity?.SetTag("request.kind", requestKind);

        try
        {
            var response = await next(cancellationToken);
            var outcome = ResolveOutcome(response);
            activity?.SetTag("outcome", outcome);

            if (outcome is "failure" or "error")
            {
                activity?.SetStatus(ActivityStatusCode.Error);
            }

            return response;
        }
        catch (Exception exception)
        {
            activity?.SetTag("outcome", "error");
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
    }

    private static string ResolveRequestKind(Type requestType)
    {
        if (RequestKind.IsCommand(requestType))
            return "command";

        if (RequestKind.IsQuery(requestType))
            return "query";

        return "unknown";
    }

    private static string ResolveOutcome(TResponse response)
    {
        if (response is Result result)
            return result.IsSuccess ? "success" : "failure";

        var responseType = response?.GetType();
        if (responseType?.IsGenericType == true &&
            responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var isSuccess = (bool)responseType.GetProperty(nameof(Result.IsSuccess))!.GetValue(response)!;
            return isSuccess ? "success" : "failure";
        }

        return "success";
    }
}
