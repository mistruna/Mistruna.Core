using Microsoft.EntityFrameworkCore;

namespace Mistruna.Core.AspNetCore.ExceptionHandling;

internal static class DbUpdateViolationDetector
{
    public static bool IsForeignKeyViolation(DbUpdateException exception, out string? constraint)
    {
        var message = exception.InnerException?.Message ?? string.Empty;
        var isViolation =
            message.Contains("foreign key constraint", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("violates foreign key constraint", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("FOREIGN KEY constraint failed", StringComparison.OrdinalIgnoreCase);

        constraint = isViolation ? ExtractConstraint(message) : null;
        return isViolation;
    }

    public static bool IsUniqueViolation(DbUpdateException exception, out string? constraint)
    {
        var message = exception.InnerException?.Message ?? string.Empty;
        var isViolation =
            message.Contains("unique constraint", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase) ||
            message.Contains("UNIQUE constraint failed", StringComparison.OrdinalIgnoreCase);

        constraint = isViolation ? ExtractConstraint(message) : null;
        return isViolation;
    }

    private static string? ExtractConstraint(string message)
    {
        var marker = "constraint '";
        var start = message.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
        if (start < 0)
        {
            return null;
        }

        start += marker.Length;
        var end = message.IndexOf('\'', start);
        return end > start ? message[start..end] : null;
    }
}
