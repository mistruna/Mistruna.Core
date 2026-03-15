namespace Mistruna.Core.Extensions;

/// <summary>
/// Provides extension methods for working with DateTime.
/// </summary>
public static class DateTimeExtensions
{
    /// <summary>
    /// Gets the start of the day.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The start of the day (00:00:00).</returns>
    public static DateTime StartOfDay(this DateTime date)
        => date.Date;

    /// <summary>
    /// Gets the end of the day.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The end of the day (23:59:59.999).</returns>
    public static DateTime EndOfDay(this DateTime date)
        => date.Date.AddDays(1).AddTicks(-1);

    /// <summary>
    /// Gets the start of the week (Monday).
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The start of the week.</returns>
    public static DateTime StartOfWeek(this DateTime date)
    {
        var diff = (7 + (date.DayOfWeek - DayOfWeek.Monday)) % 7;
        return date.AddDays(-diff).Date;
    }

    /// <summary>
    /// Gets the end of the week (Sunday).
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The end of the week.</returns>
    public static DateTime EndOfWeek(this DateTime date)
        => date.StartOfWeek().AddDays(7).AddTicks(-1);

    /// <summary>
    /// Gets the start of the month.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The start of the month.</returns>
    public static DateTime StartOfMonth(this DateTime date)
        => new(date.Year, date.Month, 1, 0, 0, 0, date.Kind);

    /// <summary>
    /// Gets the end of the month.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The end of the month.</returns>
    public static DateTime EndOfMonth(this DateTime date)
        => date.StartOfMonth().AddMonths(1).AddTicks(-1);

    /// <summary>
    /// Gets the start of the year.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The start of the year.</returns>
    public static DateTime StartOfYear(this DateTime date)
        => new(date.Year, 1, 1, 0, 0, 0, date.Kind);

    /// <summary>
    /// Gets the end of the year.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The end of the year.</returns>
    public static DateTime EndOfYear(this DateTime date)
        => new DateTime(date.Year, 12, 31, 23, 59, 59, 999, date.Kind);

    /// <summary>
    /// Checks if the date is a weekend.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>True if the date is a weekend.</returns>
    public static bool IsWeekend(this DateTime date)
        => date.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday;

    /// <summary>
    /// Checks if the date is a weekday.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>True if the date is a weekday.</returns>
    public static bool IsWeekday(this DateTime date)
        => !date.IsWeekend();

    /// <summary>
    /// Gets the age in years from a birth date.
    /// </summary>
    /// <param name="birthDate">The birth date.</param>
    /// <returns>The age in years.</returns>
    public static int GetAge(this DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age))
            age--;
        return age;
    }

    /// <summary>
    /// Checks if the date is between two dates.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <param name="start">The start date.</param>
    /// <param name="end">The end date.</param>
    /// <returns>True if the date is between start and end.</returns>
    public static bool IsBetween(this DateTime date, DateTime start, DateTime end)
        => date >= start && date <= end;

    /// <summary>
    /// Converts to Unix timestamp (seconds since 1970-01-01).
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>The Unix timestamp.</returns>
    public static long ToUnixTimestamp(this DateTime date)
        => new DateTimeOffset(date).ToUnixTimeSeconds();

    /// <summary>
    /// Creates a DateTime from Unix timestamp.
    /// </summary>
    /// <param name="timestamp">The Unix timestamp.</param>
    /// <returns>The DateTime.</returns>
    public static DateTime FromUnixTimestamp(long timestamp)
        => DateTimeOffset.FromUnixTimeSeconds(timestamp).DateTime;

    /// <summary>
    /// Adds business days to a date (excludes weekends).
    /// </summary>
    /// <param name="date">The date.</param>
    /// <param name="days">The number of business days to add.</param>
    /// <returns>The resulting date.</returns>
    public static DateTime AddBusinessDays(this DateTime date, int days)
    {
        var sign = Math.Sign(days);
        var remaining = Math.Abs(days);

        while (remaining > 0)
        {
            date = date.AddDays(sign);
            if (date.IsWeekday())
                remaining--;
        }

        return date;
    }

    /// <summary>
    /// Gets a human-readable relative time string.
    /// </summary>
    /// <param name="date">The date.</param>
    /// <returns>A relative time string (e.g., "2 hours ago").</returns>
    public static string ToRelativeTime(this DateTime date)
    {
        var now = DateTime.UtcNow;
        var diff = now - date;

        if (diff.TotalSeconds < 60)
            return "just now";
        if (diff.TotalMinutes < 60)
            return $"{(int)diff.TotalMinutes} minute{(diff.TotalMinutes >= 2 ? "s" : "")} ago";
        if (diff.TotalHours < 24)
            return $"{(int)diff.TotalHours} hour{(diff.TotalHours >= 2 ? "s" : "")} ago";
        if (diff.TotalDays < 7)
            return $"{(int)diff.TotalDays} day{(diff.TotalDays >= 2 ? "s" : "")} ago";
        if (diff.TotalDays < 30)
            return $"{(int)(diff.TotalDays / 7)} week{(diff.TotalDays >= 14 ? "s" : "")} ago";
        if (diff.TotalDays < 365)
            return $"{(int)(diff.TotalDays / 30)} month{(diff.TotalDays >= 60 ? "s" : "")} ago";

        return $"{(int)(diff.TotalDays / 365)} year{(diff.TotalDays >= 730 ? "s" : "")} ago";
    }
}
