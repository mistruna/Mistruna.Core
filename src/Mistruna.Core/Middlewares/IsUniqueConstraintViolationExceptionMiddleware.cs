using Microsoft.EntityFrameworkCore;

namespace Mistruna.Core.Middlewares;

/// <summary>
/// Helper middleware for detecting unique constraint violations.
/// </summary>
/// <remarks>
/// Detects unique-constraint violations by inspecting <see cref="DbUpdateException.InnerException"/> message.
/// Covers SQL Server ("UNIQUE KEY constraint", "duplicate key"), PostgreSQL ("unique constraint", "duplicate key value"),
/// and SQLite ("UNIQUE constraint failed") without requiring provider-specific assemblies.
/// </remarks>
public static class IsUniqueConstraintViolationExceptionMiddleware
{
    /// <summary>
    /// Checks if a DbUpdateException is caused by a unique constraint violation.
    /// </summary>
    /// <param name="ex">The DbUpdateException to check.</param>
    /// <param name="violatedConstraint">The name of the violated constraint if found.</param>
    /// <returns>True if a unique constraint violation was detected, false otherwise.</returns>
    public static bool CheckUniqueConstraintViolation(DbUpdateException ex, out string violatedConstraint)
    {
        violatedConstraint = null;

        if (!IsUniqueConstraintViolation(ex))
            return false;

        var errorMessage = ex.InnerException?.Message ?? string.Empty;

        var constraintMatch = errorMessage.IndexOf("constraint '", StringComparison.OrdinalIgnoreCase);
        if (constraintMatch != -1)
        {
            var startIdx = constraintMatch + "constraint '".Length;
            var endIdx = errorMessage.IndexOf("'", startIdx, StringComparison.Ordinal);
            if (endIdx != -1)
            {
                violatedConstraint = errorMessage.Substring(startIdx, endIdx - startIdx);
            }
        }

        return true;
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        var inner = ex.InnerException;
        if (inner is null) return false;

        var typeName = inner.GetType().Name;
        return (typeName == "SqlException" && inner.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase))
            || inner.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
            || inner.Message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase)
            || inner.Message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase);
    }
}
