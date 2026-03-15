namespace Mistruna.Core.Contracts.Attributes;

/// <summary>
/// Specifies the precision and scale for decimal properties.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class PrecisionAttribute : Attribute
{
    /// <summary>
    /// Total number of digits (including digits after decimal point).
    /// </summary>
    public int Precision { get; }

    /// <summary>
    /// Number of digits after the decimal point.
    /// </summary>
    public int Scale { get; }

    /// <summary>
    /// Creates a new PrecisionAttribute with the specified precision and scale.
    /// </summary>
    /// <param name="precision">Total number of digits.</param>
    /// <param name="scale">Number of decimal places.</param>
    public PrecisionAttribute(int precision, int scale)
    {
        if (precision < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(precision), "Precision must be at least 1.");
        }

        if (scale < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be non-negative.");
        }

        if (scale > precision)
        {
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale cannot be greater than precision.");
        }

        Precision = precision;
        Scale = scale;
    }

    /// <summary>
    /// Gets the maximum value allowed for the specified precision and scale.
    /// </summary>
    public decimal GetMaxValue()
    {
        var integerDigits = Precision - Scale;
        var maxIntPart = (decimal)Math.Pow(10, integerDigits) - 1;
        var maxFracPart = Scale > 0 ? (decimal)(Math.Pow(10, Scale) - 1) / (decimal)Math.Pow(10, Scale) : 0m;
        return maxIntPart + maxFracPart;
    }

    /// <summary>
    /// Gets the minimum value allowed (negative of max value).
    /// </summary>
    public decimal GetMinValue() => -GetMaxValue();
}
