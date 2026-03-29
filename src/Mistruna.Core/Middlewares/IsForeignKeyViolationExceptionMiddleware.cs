using Microsoft.EntityFrameworkCore;

namespace Mistruna.Core.Middlewares;

/// <summary>
/// Helper middleware for detecting foreign-key constraint violations.
/// </summary>
/// <remarks>
/// Detects foreign-key violations by inspecting <see cref="DbUpdateException.InnerException"/> message.
/// Covers SQL Server ("FOREIGN KEY constraint"), PostgreSQL ("violates foreign key constraint"),
/// and SQLite ("FOREIGN KEY constraint failed") without requiring provider-specific assemblies.
/// </remarks>
public static class IsForeignKeyViolationExceptionMiddleware
{
    /// <summary>
    /// Checks if a DbUpdateException is caused by a foreign-key constraint violation.
    /// </summary>
    /// <param name="ex">The DbUpdateException to check.</param>
    /// <param name="referencedObject">The referenced object name if extractable from the message.</param>
    /// <returns>True if a foreign-key violation was detected, false otherwise.</returns>
    public static bool CheckForeignKeyViolation(DbUpdateException ex, out string referencedObject)
    {
        referencedObject = null;

        if (!IsForeignKeyViolation(ex))
        {
            return false;
        }

        var errorMessage = ex.InnerException?.Message ?? string.Empty;

        var startIdx = errorMessage.IndexOf("referenced by '", StringComparison.Ordinal);
        if (startIdx != -1)
        {
            startIdx += "referenced by '".Length;
            var endIdx = errorMessage.IndexOf("'", startIdx, StringComparison.Ordinal);
            if (endIdx != -1)
            {
                referencedObject = errorMessage.Substring(startIdx, endIdx - startIdx);
            }
        }

        return true;
    }

    private static bool IsForeignKeyViolation(DbUpdateException ex)
    {
        var inner = ex.InnerException;
        if (inner is null) return false;

        var typeName = inner.GetType().Name;
        return (typeName == "SqlException" && inner.Message.Contains("FOREIGN KEY", StringComparison.OrdinalIgnoreCase))
            || inner.Message.Contains("foreign key constraint", StringComparison.OrdinalIgnoreCase)
            || inner.Message.Contains("violates foreign key constraint", StringComparison.OrdinalIgnoreCase)
            || inner.Message.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase);
    }
}
