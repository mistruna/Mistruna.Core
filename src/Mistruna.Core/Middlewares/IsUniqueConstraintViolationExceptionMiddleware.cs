using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Mistruna.Core.Middlewares;

/// <summary>
/// Helper middleware for detecting SQL Server unique constraint violations.
/// </summary>
public static class IsUniqueConstraintViolationExceptionMiddleware
{
    /// <summary>
    /// Checks if a DbUpdateException is caused by a unique constraint violation.
    /// </summary>
    /// <param name="ex">The DbUpdateException to check.</param>
    /// <param name="violatedConstraint">The name of the violated constraint if found.</param>
    /// <returns>True if a unique constraint violation was detected, false otherwise.</returns>
    /// <remarks>
    /// SQL Server error code 2627 indicates a unique constraint violation.
    /// </remarks>
    public static bool CheckUniqueConstraintViolation(DbUpdateException ex, out string violatedConstraint)
    {
        violatedConstraint = null;

        if (ex.InnerException is SqlException sqlException && sqlException.Number == 2627)
        {
            var errorMessage = sqlException.Message;

            // Try to extract the constraint name from the error message
            // Message format: "Violation of PRIMARY KEY constraint 'PK_TableName'. Cannot insert duplicate key in object 'dbo.TableName'."
            // Or: "Violation of UNIQUE KEY constraint 'UQ_ConstraintName'. Cannot insert duplicate key..."
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

        return false;
    }
}
