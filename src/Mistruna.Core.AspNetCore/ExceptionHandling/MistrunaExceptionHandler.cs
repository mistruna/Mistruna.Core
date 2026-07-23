using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mistruna.Core.AspNetCore.Results;
using Mistruna.Core.Exceptions;

namespace Mistruna.Core.AspNetCore.ExceptionHandling;

public sealed class MistrunaExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<MistrunaExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var mapping = Map(exception);

        if (mapping.StatusCode >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception. TraceId: {TraceId}", httpContext.TraceIdentifier);
        }
        else
        {
            logger.LogWarning(exception, "Request failed with {ErrorCode}. TraceId: {TraceId}",
                mapping.ErrorCode, httpContext.TraceIdentifier);
        }

        var problem = exception.ToProblemDetails(
            mapping.StatusCode,
            mapping.Title,
            mapping.ErrorCode,
            httpContext.TraceIdentifier,
            mapping.Details,
            mapping.Detail);

        httpContext.Response.StatusCode = mapping.StatusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = problem,
            Exception = exception
        });
    }

    private static ExceptionMapping Map(Exception exception)
    {
        return exception switch
        {
            ValidationException validation => new(
                StatusCodes.Status400BadRequest,
                "Validation failed",
                "VALIDATION_ERROR",
                validation.Errors.Select(error => new
                {
                    error.PropertyName,
                    error.ErrorMessage,
                    error.ErrorCode
                }).ToArray()),
            BadRequestException badRequest => FromCore(
                badRequest, StatusCodes.Status400BadRequest, "Bad request"),
            NotFoundException notFound => FromCore(
                notFound, StatusCodes.Status404NotFound, "Resource not found"),
            UnauthorizedAccessException => new(
                StatusCodes.Status401Unauthorized, "Unauthorized", "UNAUTHORIZED"),
            ForbiddenAccessException forbidden => FromCore(
                forbidden, StatusCodes.Status403Forbidden, "Forbidden"),
            ConflictException conflict => FromCore(
                conflict, StatusCodes.Status409Conflict, "Conflict"),
            TimeoutException => new(
                StatusCodes.Status408RequestTimeout, "Request timed out", "TIMEOUT"),
            DbUpdateException dbUpdate when
                DbUpdateViolationDetector.IsForeignKeyViolation(dbUpdate, out var foreignKey) => new(
                    StatusCodes.Status409Conflict,
                    "Foreign key constraint violation",
                    "FOREIGN_KEY_VIOLATION",
                    foreignKey,
                    "The operation conflicts with a related resource."),
            DbUpdateException dbUpdate when
                DbUpdateViolationDetector.IsUniqueViolation(dbUpdate, out var uniqueConstraint) => new(
                    StatusCodes.Status409Conflict,
                    "Unique constraint violation",
                    "UNIQUE_CONSTRAINT_VIOLATION",
                    uniqueConstraint,
                    "A resource with the provided values already exists."),
            CoreException core => FromCore(
                core, StatusCodes.Status400BadRequest, "Request failed"),
            _ => new(
                StatusCodes.Status500InternalServerError,
                "Internal server error",
                "INTERNAL_ERROR",
                Detail: "An unexpected error occurred.")
        };
    }

    private static ExceptionMapping FromCore(CoreException exception, int statusCode, string title) =>
        new(statusCode, title, exception.ErrorCode, exception.Details);

    private sealed record ExceptionMapping(
        int StatusCode,
        string Title,
        string ErrorCode,
        object? Details = null,
        string? Detail = null);
}
