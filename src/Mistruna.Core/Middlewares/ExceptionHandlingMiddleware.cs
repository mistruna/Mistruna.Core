using System.Net.Mime;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Mistruna.Core.Contracts.Base.Responses;
using Mistruna.Core.Exceptions;

namespace Mistruna.Core.Middlewares;

/// <summary>
/// Middleware that provides centralized exception handling for ASP.NET Core applications.
/// Converts exceptions to appropriate HTTP responses with consistent error format.
/// </summary>
/// <remarks>
/// This middleware catches exceptions thrown by downstream middleware and converts them
/// to appropriate HTTP responses. It supports the following exception types:
/// <list type="bullet">
///     <item><description><see cref="ValidationException"/> - Returns HTTP 400 Bad Request</description></item>
///     <item><description><see cref="BadRequestException"/> - Returns HTTP 400 Bad Request</description></item>
///     <item><description><see cref="NotFoundException"/> - Returns HTTP 404 Not Found</description></item>
///     <item><description><see cref="UnauthorizedAccessException"/> - Returns HTTP 401 Unauthorized</description></item>
///     <item><description><see cref="ForbiddenAccessException"/> - Returns HTTP 403 Forbidden</description></item>
///     <item><description><see cref="TimeoutException"/> - Returns HTTP 408 Request Timeout</description></item>
///     <item><description><see cref="ConflictException"/> - Returns HTTP 409 Conflict</description></item>
///     <item><description><see cref="NotImplementedException"/> - Returns HTTP 501 Not Implemented</description></item>
///     <item><description><see cref="DbUpdateException"/> - Returns HTTP 500 Internal Server Error</description></item>
/// </list>
///
/// Register this middleware in your request pipeline:
/// <code>
/// app.UseMiddleware&lt;ExceptionHandlingMiddleware&gt;();
/// </code>
/// </remarks>
public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    /// <summary>
    /// Invokes the middleware, catching and handling any exceptions that occur.
    /// </summary>
    /// <param name="context">The HTTP context for the current request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage, e.ErrorCode });
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, "Validation error.", errors, "VALIDATION_ERROR");
        }
        catch (BadRequestException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message, ex.Details, ex.ErrorCode);
        }
        catch (NotFoundException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status404NotFound, ex.Message, ex.Details, ex.ErrorCode);
        }
        catch (UnauthorizedAccessException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status401Unauthorized, "Unauthorized.", ex.Message, "UNAUTHORIZED");
        }
        catch (ForbiddenAccessException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status403Forbidden, ex.Message, ex.Details, ex.ErrorCode);
        }
        catch (TimeoutException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status408RequestTimeout, "Request timed out.", ex.Message, "TIMEOUT");
        }
        catch (DbUpdateException ex)
            when (IsForeignKeyViolationExceptionMiddleware.CheckForeignKeyViolation(ex, out var referencedObject))
        {
            await HandleExceptionAsync(
                context,
                StatusCodes.Status409Conflict,
                "Unable to delete object. It is referenced by another entity.",
                referencedObject,
                "FOREIGN_KEY_VIOLATION");
        }
        catch (DbUpdateException ex)
            when (IsUniqueConstraintViolationExceptionMiddleware.CheckUniqueConstraintViolation(ex, out var violatedConstraint))
        {
            logger.LogWarning(ex, "Unique constraint violation: {ConstraintName}", violatedConstraint);
            await HandleExceptionAsync(
                context,
                StatusCodes.Status409Conflict,
                "The record with the provided values already exists.",
                violatedConstraint,
                "UNIQUE_CONSTRAINT_VIOLATION");
        }
        catch (DbUpdateException ex)
        {
            logger.LogError(ex, "Database error occurred.");
            await HandleExceptionAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Database error.",
                ex.InnerException?.Message ?? ex.Message,
                "DATABASE_ERROR");
        }
        catch (OperationCanceledException)
        {
            context.Response.StatusCode = 499;
        }
        catch (ConflictException ex)
        {
            await HandleExceptionAsync(context, StatusCodes.Status409Conflict, ex.Message, ex.Details, ex.ErrorCode);
        }
        catch (NotImplementedException)
        {
            await HandleExceptionAsync(context, StatusCodes.Status501NotImplemented, "Not implemented.", errorCode: "NOT_IMPLEMENTED");
        }
        catch (CoreException ex)
        {
            logger.LogWarning(ex, "Core exception occurred: {ErrorCode}", ex.ErrorCode);
            await HandleExceptionAsync(context, StatusCodes.Status400BadRequest, ex.Message, ex.Details, ex.ErrorCode);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred.");
            await HandleExceptionAsync(context, StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.", errorCode: "INTERNAL_ERROR");
        }
    }

    private static Task HandleExceptionAsync(
        HttpContext context,
        int statusCode,
        string message,
        object? details = null,
        string? errorCode = null)
    {
        context.Response.ContentType = MediaTypeNames.Application.Json;
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse
        {
            Message = message,
            ErrorCode = errorCode,
            Details = details,
            TraceId = context.TraceIdentifier,
            Timestamp = DateTime.UtcNow
        };

        return context.Response.WriteAsJsonAsync(response);
    }
}
