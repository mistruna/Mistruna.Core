namespace Mistruna.Core.Exceptions;

/// <summary>
/// Exception thrown when the user is authenticated but lacks the required permissions.
/// This exception is typically handled by middleware to return HTTP 403 status code.
/// </summary>
/// <remarks>
/// Use this exception when the user is authenticated but doesn't have the required
/// permissions or roles to perform the requested action. For unauthenticated users,
/// use <see cref="UnauthorizedAccessException"/> instead.
/// </remarks>
/// <example>
/// <code>
/// if (!user.HasRole("Admin"))
///     throw new ForbiddenAccessException("Only administrators can perform this action");
/// </code>
/// </example>
public class ForbiddenAccessException : CoreException
{
    private const string DefaultErrorCode = "FORBIDDEN";

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ForbiddenAccessException(string message = "Access denied")
        : base(DefaultErrorCode, message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForbiddenAccessException"/> class with a custom error code.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The custom error code.</param>
    public ForbiddenAccessException(string message, string errorCode)
        : base(errorCode, message)
    {
    }

    /// <summary>
    /// Creates a ForbiddenAccessException for missing role.
    /// </summary>
    /// <param name="requiredRole">The required role.</param>
    public static ForbiddenAccessException ForRole(string requiredRole) =>
        new($"Access denied. Required role: {requiredRole}", "FORBIDDEN_ROLE");

    /// <summary>
    /// Creates a ForbiddenAccessException for missing permission.
    /// </summary>
    /// <param name="permission">The required permission.</param>
    public static ForbiddenAccessException ForPermission(string permission) =>
        new($"Access denied. Required permission: {permission}", "FORBIDDEN_PERMISSION");
}
