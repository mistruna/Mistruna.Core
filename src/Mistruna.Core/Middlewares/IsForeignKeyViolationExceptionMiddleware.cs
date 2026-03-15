using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Mistruna.Core.Middlewares;

public static class IsForeignKeyViolationExceptionMiddleware
{
    public static bool CheckForeignKeyViolation(DbUpdateException ex, out string referencedObject)
    {
        referencedObject = null;

        if (ex.InnerException is SqlException sqlException && sqlException.Number == 547)
        {
            var errorMessage = sqlException.Message;

            var startIdx = errorMessage.IndexOf("referenced by '", StringComparison.Ordinal)
                           + "referenced by '".Length;
            var endIdx = errorMessage.IndexOf("'", startIdx, StringComparison.Ordinal);

            if (startIdx != -1 && endIdx != -1)
            {
                referencedObject = errorMessage.Substring(startIdx, endIdx - startIdx);
            }

            return true;
        }

        return false;
    }
}
