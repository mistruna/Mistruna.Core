using Microsoft.AspNetCore.Mvc;

namespace Mistruna.Core.AspNetCore.Results;

public static class ProblemDetailsExtensions
{
    public static ProblemDetails ToProblemDetails(
        this Exception exception,
        int statusCode,
        string title,
        string errorCode,
        string traceId,
        object? details = null,
        string? detail = null)
    {
        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail ?? exception.Message
        };

        problem.Extensions["errorCode"] = errorCode;
        problem.Extensions["traceId"] = traceId;

        if (details is not null)
        {
            problem.Extensions["details"] = details;
        }

        return problem;
    }
}
