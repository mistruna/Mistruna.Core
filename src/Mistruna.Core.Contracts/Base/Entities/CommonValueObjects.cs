using System.Text.RegularExpressions;

namespace Mistruna.Core.Contracts.Base.Entities;

/// <summary>
/// Represents an email address value object.
/// </summary>
public sealed class Email : ValueObject
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    /// <summary>
    /// Gets the email address value.
    /// </summary>
    public string Value { get; }

    private Email(string value)
    {
        Value = value.ToLowerInvariant();
    }

    /// <summary>
    /// Creates a new Email instance.
    /// </summary>
    /// <param name="email">The email address string.</param>
    /// <returns>A new Email value object.</returns>
    /// <exception cref="ArgumentException">Thrown when the email format is invalid.</exception>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));

        if (!EmailRegex.IsMatch(email))
            throw new ArgumentException("Invalid email format.", nameof(email));

        return new Email(email);
    }

    /// <summary>
    /// Tries to create a new Email instance.
    /// </summary>
    /// <param name="email">The email address string.</param>
    /// <param name="result">The created Email, or null if invalid.</param>
    /// <returns>True if the email was valid, otherwise false.</returns>
    public static bool TryCreate(string email, out Email? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(email) || !EmailRegex.IsMatch(email))
            return false;

        result = new Email(email);
        return true;
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Implicit conversion to string.
    /// </summary>
    public static implicit operator string(Email email) => email.Value;
}

/// <summary>
/// Represents a phone number value object.
/// </summary>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex PhoneRegex = new(
        @"^\+?[1-9]\d{1,14}$",
        RegexOptions.Compiled);

    /// <summary>
    /// Gets the phone number value (digits only, with optional leading +).
    /// </summary>
    public string Value { get; }

    private PhoneNumber(string value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new PhoneNumber instance.
    /// </summary>
    /// <param name="phoneNumber">The phone number string.</param>
    /// <returns>A new PhoneNumber value object.</returns>
    /// <exception cref="ArgumentException">Thrown when the phone number format is invalid.</exception>
    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));

        // Remove common formatting characters
        var normalized = Regex.Replace(phoneNumber, @"[\s\-\(\)\.]", "");

        if (!PhoneRegex.IsMatch(normalized))
            throw new ArgumentException("Invalid phone number format.", nameof(phoneNumber));

        return new PhoneNumber(normalized);
    }

    /// <summary>
    /// Tries to create a new PhoneNumber instance.
    /// </summary>
    /// <param name="phoneNumber">The phone number string.</param>
    /// <param name="result">The created PhoneNumber, or null if invalid.</param>
    /// <returns>True if the phone number was valid, otherwise false.</returns>
    public static bool TryCreate(string phoneNumber, out PhoneNumber? result)
    {
        result = null;

        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;

        var normalized = Regex.Replace(phoneNumber, @"[\s\-\(\)\.]", "");

        if (!PhoneRegex.IsMatch(normalized))
            return false;

        result = new PhoneNumber(normalized);
        return true;
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>
    /// Implicit conversion to string.
    /// </summary>
    public static implicit operator string(PhoneNumber phone) => phone.Value;
}

/// <summary>
/// Represents a money value object.
/// </summary>
public sealed class Money : ValueObject, IComparable<Money>
{
    /// <summary>
    /// Gets the amount.
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Gets the currency code.
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>
    /// Creates a new Money instance.
    /// </summary>
    /// <param name="amount">The amount.</param>
    /// <param name="currency">The currency code (e.g., USD, EUR).</param>
    /// <returns>A new Money value object.</returns>
    public static Money Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));

        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter code.", nameof(currency));

        return new Money(Math.Round(amount, 2), currency);
    }

    /// <summary>
    /// Creates a zero Money instance.
    /// </summary>
    /// <param name="currency">The currency code.</param>
    /// <returns>A Money with zero amount.</returns>
    public static Money Zero(string currency) => Create(0, currency);

    /// <summary>
    /// Adds two Money instances.
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return Create(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two Money instances.
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return Create(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies Money by a scalar.
    /// </summary>
    public static Money operator *(Money money, decimal multiplier)
    {
        return Create(money.Amount * multiplier, money.Currency);
    }

    /// <summary>
    /// Divides Money by a scalar.
    /// </summary>
    public static Money operator /(Money money, decimal divisor)
    {
        if (divisor == 0)
            throw new DivideByZeroException();

        return Create(money.Amount / divisor, money.Currency);
    }

    /// <summary>
    /// Less than operator.
    /// </summary>
    public static bool operator <(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount < right.Amount;
    }

    /// <summary>
    /// Greater than operator.
    /// </summary>
    public static bool operator >(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount > right.Amount;
    }

    /// <summary>
    /// Less than or equal operator.
    /// </summary>
    public static bool operator <=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount <= right.Amount;
    }

    /// <summary>
    /// Greater than or equal operator.
    /// </summary>
    public static bool operator >=(Money left, Money right)
    {
        EnsureSameCurrency(left, right);
        return left.Amount >= right.Amount;
    }

    /// <inheritdoc />
    public int CompareTo(Money? other)
    {
        if (other is null)
            return 1;

        EnsureSameCurrency(this, other);
        return Amount.CompareTo(other.Amount);
    }

    private static void EnsureSameCurrency(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException(
                $"Cannot perform operation on different currencies: {left.Currency} and {right.Currency}");
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Amount:N2} {Currency}";
}

/// <summary>
/// Represents an address value object.
/// </summary>
public sealed class Address : ValueObject
{
    /// <summary>
    /// Gets the street address.
    /// </summary>
    public string Street { get; }

    /// <summary>
    /// Gets the city.
    /// </summary>
    public string City { get; }

    /// <summary>
    /// Gets the state or province.
    /// </summary>
    public string? State { get; }

    /// <summary>
    /// Gets the postal code.
    /// </summary>
    public string PostalCode { get; }

    /// <summary>
    /// Gets the country.
    /// </summary>
    public string Country { get; }

    private Address(string street, string city, string? state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    /// <summary>
    /// Creates a new Address instance.
    /// </summary>
    public static Address Create(
        string street,
        string city,
        string postalCode,
        string country,
        string? state = null)
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be null or empty.", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be null or empty.", nameof(city));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be null or empty.", nameof(postalCode));

        if (string.IsNullOrWhiteSpace(country))
            throw new ArgumentException("Country cannot be null or empty.", nameof(country));

        return new Address(street.Trim(), city.Trim(), state?.Trim(), postalCode.Trim(), country.Trim());
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Street;
        yield return City;
        yield return State;
        yield return PostalCode;
        yield return Country;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var parts = new List<string> { Street, City };

        if (!string.IsNullOrWhiteSpace(State))
            parts.Add(State!);

        parts.Add(PostalCode);
        parts.Add(Country);

        return string.Join(", ", parts);
    }
}

/// <summary>
/// Represents a date range value object.
/// </summary>
public sealed class DateRange : ValueObject
{
    /// <summary>
    /// Gets the start date.
    /// </summary>
    public DateTime Start { get; }

    /// <summary>
    /// Gets the end date.
    /// </summary>
    public DateTime End { get; }

    /// <summary>
    /// Gets the duration of the range.
    /// </summary>
    public TimeSpan Duration => End - Start;

    private DateRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>
    /// Creates a new DateRange instance.
    /// </summary>
    public static DateRange Create(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ArgumentException("End date must be greater than or equal to start date.", nameof(end));

        return new DateRange(start, end);
    }

    /// <summary>
    /// Checks if a date falls within this range.
    /// </summary>
    public bool Contains(DateTime date)
    {
        return date >= Start && date <= End;
    }

    /// <summary>
    /// Checks if this range overlaps with another range.
    /// </summary>
    public bool Overlaps(DateRange other)
    {
        return Start <= other.End && End >= other.Start;
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Start:d} - {End:d}";
}
