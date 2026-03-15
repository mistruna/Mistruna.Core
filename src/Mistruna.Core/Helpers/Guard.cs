using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Mistruna.Core.Helpers;

/// <summary>
/// Provides guard clause methods for validating method arguments.
/// </summary>
/// <remarks>
/// Use these methods at the beginning of your methods to validate preconditions.
/// All methods throw appropriate exceptions when validation fails.
/// </remarks>
/// <example>
/// <code>
/// public void UpdateUser(User user, string email)
/// {
///     Guard.AgainstNull(user);
///     Guard.AgainstNullOrEmpty(email);
///     Guard.AgainstInvalidEmail(email);
///     // method implementation
/// }
/// </code>
/// </example>
public static class Guard
{
    /// <summary>
    /// Throws if the value is null.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    public static void AgainstNull(
        [NotNull] object? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
    }

    /// <summary>
    /// Throws if the value is null and returns the non-null value.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <returns>The non-null value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when value is null.</exception>
    [return: NotNull]
    public static T AgainstNullWithReturn<T>(
        [NotNull] T? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : class
    {
        if (value is null)
            throw new ArgumentNullException(parameterName);
        return value;
    }

    /// <summary>
    /// Throws if the string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when string is null or empty.</exception>
    public static void AgainstNullOrEmpty(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("Value cannot be null or empty.", parameterName);
    }

    /// <summary>
    /// Throws if the string is null, empty, or contains only whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when string is null, empty, or whitespace.</exception>
    public static void AgainstNullOrWhiteSpace(
        [NotNull] string? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value cannot be null, empty, or whitespace.", parameterName);
    }

    /// <summary>
    /// Throws if the collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="value">The collection to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when collection is null or empty.</exception>
    public static void AgainstNullOrEmpty<T>(
        [NotNull] IEnumerable<T>? value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value is null || !value.Any())
            throw new ArgumentException("Collection cannot be null or empty.", parameterName);
    }

    /// <summary>
    /// Throws if the value is the default value for its type.
    /// </summary>
    /// <typeparam name="T">The type of the value.</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when value is the default value.</exception>
    public static void AgainstDefault<T>(
        T value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : struct
    {
        if (EqualityComparer<T>.Default.Equals(value, default))
            throw new ArgumentException("Value cannot be the default value.", parameterName);
    }

    /// <summary>
    /// Throws if the GUID is empty.
    /// </summary>
    /// <param name="value">The GUID to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when GUID is empty.</exception>
    public static void AgainstEmptyGuid(
        Guid value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("GUID cannot be empty.", parameterName);
    }

    /// <summary>
    /// Throws if the value is out of range.
    /// </summary>
    /// <typeparam name="T">The type of the value (must be comparable).</typeparam>
    /// <param name="value">The value to check.</param>
    /// <param name="min">The minimum allowed value (inclusive).</param>
    /// <param name="max">The maximum allowed value (inclusive).</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is out of range.</exception>
    public static void AgainstOutOfRange<T>(
        T value,
        T min,
        T max,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
        where T : IComparable<T>
    {
        if (value.CompareTo(min) < 0 || value.CompareTo(max) > 0)
            throw new ArgumentOutOfRangeException(parameterName, value, $"Value must be between {min} and {max}.");
    }

    /// <summary>
    /// Throws if the value is negative.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative.</exception>
    public static void AgainstNegative(
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
    }

    /// <summary>
    /// Throws if the value is negative or zero.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative or zero.</exception>
    public static void AgainstNegativeOrZero(
        int value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(parameterName, value, "Value must be positive.");
    }

    /// <summary>
    /// Throws if the value is negative.
    /// </summary>
    /// <param name="value">The value to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when value is negative.</exception>
    public static void AgainstNegative(
        decimal value,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value < 0)
            throw new ArgumentOutOfRangeException(parameterName, value, "Value cannot be negative.");
    }

    /// <summary>
    /// Throws if the string exceeds the maximum length.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when string exceeds max length.</exception>
    public static void AgainstLengthExceeded(
        string? value,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        if (value?.Length > maxLength)
            throw new ArgumentException($"String length cannot exceed {maxLength} characters.", parameterName);
    }

    /// <summary>
    /// Throws if the string length is not within the specified range.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="minLength">The minimum allowed length.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when string length is out of range.</exception>
    public static void AgainstLengthOutOfRange(
        string? value,
        int minLength,
        int maxLength,
        [CallerArgumentExpression(nameof(value))] string? parameterName = null)
    {
        var length = value?.Length ?? 0;
        if (length < minLength || length > maxLength)
            throw new ArgumentException(
                $"String length must be between {minLength} and {maxLength} characters.",
                parameterName);
    }

    /// <summary>
    /// Throws if the condition is true.
    /// </summary>
    /// <param name="condition">The condition to check.</param>
    /// <param name="message">The exception message.</param>
    /// <param name="parameterName">The name of the parameter (optional).</param>
    /// <exception cref="ArgumentException">Thrown when condition is true.</exception>
    public static void Against(
        bool condition,
        string message,
        string? parameterName = null)
    {
        if (condition)
            throw new ArgumentException(message, parameterName);
    }

    /// <summary>
    /// Throws if the email format is invalid.
    /// </summary>
    /// <param name="email">The email to validate.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when email format is invalid.</exception>
    public static void AgainstInvalidEmail(
        string? email,
        [CallerArgumentExpression(nameof(email))] string? parameterName = null)
    {
        AgainstNullOrWhiteSpace(email, parameterName);

        if (!email!.Contains('@') || !email.Contains('.'))
            throw new ArgumentException("Invalid email format.", parameterName);
    }

    /// <summary>
    /// Throws if the date is in the past.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when date is in the past.</exception>
    public static void AgainstPastDate(
        DateTime date,
        [CallerArgumentExpression(nameof(date))] string? parameterName = null)
    {
        if (date < DateTime.UtcNow)
            throw new ArgumentException("Date cannot be in the past.", parameterName);
    }

    /// <summary>
    /// Throws if the date is in the future.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="parameterName">The name of the parameter (auto-captured).</param>
    /// <exception cref="ArgumentException">Thrown when date is in the future.</exception>
    public static void AgainstFutureDate(
        DateTime date,
        [CallerArgumentExpression(nameof(date))] string? parameterName = null)
    {
        if (date > DateTime.UtcNow)
            throw new ArgumentException("Date cannot be in the future.", parameterName);
    }
}
