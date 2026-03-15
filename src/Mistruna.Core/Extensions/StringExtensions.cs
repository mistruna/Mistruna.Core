using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Mistruna.Core.Extensions;

/// <summary>
/// Provides extension methods for working with strings.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Returns null if the string is empty or whitespace.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>The string or null.</returns>
    public static string? NullIfEmpty(this string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value;

    /// <summary>
    /// Returns the default value if the string is null or empty.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The string or default value.</returns>
    public static string DefaultIfEmpty(this string? value, string defaultValue)
        => string.IsNullOrWhiteSpace(value) ? defaultValue : value;

    /// <summary>
    /// Truncates a string to a maximum length.
    /// </summary>
    /// <param name="value">The string to truncate.</param>
    /// <param name="maxLength">The maximum length.</param>
    /// <param name="suffix">The suffix to add if truncated (default: "...").</param>
    /// <returns>The truncated string.</returns>
    public static string Truncate(this string? value, int maxLength, string suffix = "...")
    {
        if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            return value ?? string.Empty;

        return string.Concat(value.AsSpan(0, maxLength - suffix.Length), suffix);
    }

    /// <summary>
    /// Converts a string to camelCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The camelCase string.</returns>
    public static string ToCamelCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToLowerInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts a string to PascalCase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The PascalCase string.</returns>
    public static string ToPascalCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }

    /// <summary>
    /// Converts a string to snake_case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The snake_case string.</returns>
    public static string ToSnakeCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var builder = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    builder.Append('_');
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Converts a string to kebab-case.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The kebab-case string.</returns>
    public static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        var builder = new StringBuilder();
        for (var i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0)
                    builder.Append('-');
                builder.Append(char.ToLowerInvariant(c));
            }
            else
            {
                builder.Append(c);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Removes all whitespace from a string.
    /// </summary>
    /// <param name="value">The string to process.</param>
    /// <returns>The string without whitespace.</returns>
    public static string RemoveWhitespace(this string value)
        => new(value.Where(c => !char.IsWhiteSpace(c)).ToArray());

    /// <summary>
    /// Checks if a string contains another string (case-insensitive).
    /// </summary>
    /// <param name="value">The string to search in.</param>
    /// <param name="search">The string to search for.</param>
    /// <returns>True if the string contains the search string.</returns>
    public static bool ContainsIgnoreCase(this string value, string search)
        => value.Contains(search, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Checks if a string equals another string (case-insensitive).
    /// </summary>
    /// <param name="value">The first string.</param>
    /// <param name="other">The second string.</param>
    /// <returns>True if the strings are equal.</returns>
    public static bool EqualsIgnoreCase(this string? value, string? other)
        => string.Equals(value, other, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Converts the first character of a string to uppercase.
    /// </summary>
    /// <param name="value">The string to convert.</param>
    /// <returns>The string with first character uppercase.</returns>
    public static string Capitalize(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpper(value[0], CultureInfo.CurrentCulture) + value[1..];
    }

    /// <summary>
    /// Reverses a string.
    /// </summary>
    /// <param name="value">The string to reverse.</param>
    /// <returns>The reversed string.</returns>
    public static string Reverse(this string value)
    {
        var chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }

    /// <summary>
    /// Checks if a string is a valid email address.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is a valid email.</returns>
    public static bool IsValidEmail(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        try
        {
            return Regex.IsMatch(value,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase,
                TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }

    /// <summary>
    /// Checks if a string is a valid URL.
    /// </summary>
    /// <param name="value">The string to check.</param>
    /// <returns>True if the string is a valid URL.</returns>
    public static bool IsValidUrl(this string? value)
        => Uri.TryCreate(value, UriKind.Absolute, out var result)
           && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);

    /// <summary>
    /// Masks a string, showing only the specified number of characters at the start and end.
    /// </summary>
    /// <param name="value">The string to mask.</param>
    /// <param name="showFirst">Number of characters to show at the start.</param>
    /// <param name="showLast">Number of characters to show at the end.</param>
    /// <param name="maskChar">The masking character.</param>
    /// <returns>The masked string.</returns>
    public static string Mask(this string? value, int showFirst = 2, int showLast = 2, char maskChar = '*')
    {
        if (string.IsNullOrEmpty(value))
            return value ?? string.Empty;

        if (value.Length <= showFirst + showLast)
            return new string(maskChar, value.Length);

        return string.Concat(
            value.AsSpan(0, showFirst),
            new string(maskChar, value.Length - showFirst - showLast),
            value.AsSpan(value.Length - showLast));
    }

    /// <summary>
    /// Splits a string by a separator and removes empty entries.
    /// </summary>
    /// <param name="value">The string to split.</param>
    /// <param name="separator">The separator.</param>
    /// <returns>The split string array without empty entries.</returns>
    public static string[] SplitAndTrim(this string value, params char[] separator)
        => value.Split(separator, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
