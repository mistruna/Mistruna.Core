using System.Text.RegularExpressions;

namespace Mistruna.Core.Abstractions.Entities;

/// <summary>Represents an email address.</summary>
public sealed class Email : ValueObject
{
    private static readonly Regex Pattern = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value) => Value = value.ToLowerInvariant();

    /// <summary>Gets the normalized email address.</summary>
    public string Value { get; }

    /// <summary>Creates an email address.</summary>
    public static Email Create(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(email));
        if (!Pattern.IsMatch(email))
            throw new ArgumentException("Invalid email format.", nameof(email));
        return new Email(email);
    }

    /// <summary>Attempts to create an email address.</summary>
    public static bool TryCreate(string email, out Email? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(email) || !Pattern.IsMatch(email))
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

    /// <summary>Converts an email to text.</summary>
    public static implicit operator string(Email email) => email.Value;
}

/// <summary>Represents a normalized phone number.</summary>
public sealed class PhoneNumber : ValueObject
{
    private static readonly Regex Pattern = new(@"^\+?[1-9]\d{1,14}$", RegexOptions.Compiled);
    private PhoneNumber(string value) => Value = value;

    /// <summary>Gets the normalized phone number.</summary>
    public string Value { get; }

    /// <summary>Creates a phone number.</summary>
    public static PhoneNumber Create(string phoneNumber)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number cannot be null or empty.", nameof(phoneNumber));
        var normalized = Normalize(phoneNumber);
        if (!Pattern.IsMatch(normalized))
            throw new ArgumentException("Invalid phone number format.", nameof(phoneNumber));
        return new PhoneNumber(normalized);
    }

    /// <summary>Attempts to create a phone number.</summary>
    public static bool TryCreate(string phoneNumber, out PhoneNumber? result)
    {
        result = null;
        if (string.IsNullOrWhiteSpace(phoneNumber))
            return false;
        var normalized = Normalize(phoneNumber);
        if (!Pattern.IsMatch(normalized))
            return false;
        result = new PhoneNumber(normalized);
        return true;
    }

    private static string Normalize(string value) => Regex.Replace(value, @"[\s\-\(\)\.]", "");

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public override string ToString() => Value;

    /// <summary>Converts a phone number to text.</summary>
    public static implicit operator string(PhoneNumber phone) => phone.Value;
}

/// <summary>Represents a monetary amount and currency.</summary>
public sealed class Money : ValueObject, IComparable<Money>
{
    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    /// <summary>Gets the amount.</summary>
    public decimal Amount { get; }
    /// <summary>Gets the ISO currency code.</summary>
    public string Currency { get; }

    /// <summary>Creates money rounded to two decimal places.</summary>
    public static Money Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            throw new ArgumentException("Currency cannot be null or empty.", nameof(currency));
        if (currency.Length != 3)
            throw new ArgumentException("Currency must be a 3-letter code.", nameof(currency));
        return new Money(Math.Round(amount, 2), currency);
    }

    /// <summary>Creates zero money in a currency.</summary>
    public static Money Zero(string currency) => Create(0, currency);
    /// <summary>Adds money with the same currency.</summary>
    public static Money operator +(Money left, Money right) => Create(CompareCurrency(left, right, left.Amount + right.Amount), left.Currency);
    /// <summary>Subtracts money with the same currency.</summary>
    public static Money operator -(Money left, Money right) => Create(CompareCurrency(left, right, left.Amount - right.Amount), left.Currency);
    /// <summary>Multiplies money by a scalar.</summary>
    public static Money operator *(Money money, decimal multiplier) => Create(money.Amount * multiplier, money.Currency);
    /// <summary>Divides money by a scalar.</summary>
    public static Money operator /(Money money, decimal divisor) =>
        divisor == 0 ? throw new DivideByZeroException() : Create(money.Amount / divisor, money.Currency);
    /// <summary>Compares monetary amounts.</summary>
    public static bool operator <(Money left, Money right) => CompareCurrency(left, right, left.Amount) < right.Amount;
    /// <summary>Compares monetary amounts.</summary>
    public static bool operator >(Money left, Money right) => CompareCurrency(left, right, left.Amount) > right.Amount;
    /// <summary>Compares monetary amounts.</summary>
    public static bool operator <=(Money left, Money right) => CompareCurrency(left, right, left.Amount) <= right.Amount;
    /// <summary>Compares monetary amounts.</summary>
    public static bool operator >=(Money left, Money right) => CompareCurrency(left, right, left.Amount) >= right.Amount;

    /// <inheritdoc />
    public int CompareTo(Money? other)
    {
        if (other is null)
            return 1;
        EnsureSameCurrency(this, other);
        return Amount.CompareTo(other.Amount);
    }

    private static decimal CompareCurrency(Money left, Money right, decimal value)
    {
        EnsureSameCurrency(left, right);
        return value;
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

/// <summary>Represents a postal address.</summary>
public sealed class Address : ValueObject
{
    private Address(string street, string city, string? state, string postalCode, string country)
    {
        Street = street;
        City = city;
        State = state;
        PostalCode = postalCode;
        Country = country;
    }

    /// <summary>Gets the street.</summary>
    public string Street { get; }
    /// <summary>Gets the city.</summary>
    public string City { get; }
    /// <summary>Gets the state or province.</summary>
    public string? State { get; }
    /// <summary>Gets the postal code.</summary>
    public string PostalCode { get; }
    /// <summary>Gets the country.</summary>
    public string Country { get; }

    /// <summary>Creates an address.</summary>
    public static Address Create(string street, string city, string postalCode, string country, string? state = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(street);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(postalCode);
        ArgumentException.ThrowIfNullOrWhiteSpace(country);
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
    public override string ToString() =>
        string.Join(", ", new[] { Street, City, State, PostalCode, Country }.Where(value => !string.IsNullOrWhiteSpace(value)));
}

/// <summary>Represents an inclusive date range.</summary>
public sealed class DateRange : ValueObject
{
    private DateRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    /// <summary>Gets the start.</summary>
    public DateTime Start { get; }
    /// <summary>Gets the end.</summary>
    public DateTime End { get; }
    /// <summary>Gets the duration.</summary>
    public TimeSpan Duration => End - Start;

    /// <summary>Creates a date range.</summary>
    public static DateRange Create(DateTime start, DateTime end)
    {
        if (end < start)
            throw new ArgumentException("End date must be greater than or equal to start date.", nameof(end));
        return new DateRange(start, end);
    }

    /// <summary>Returns whether a date is in the range.</summary>
    public bool Contains(DateTime date) => date >= Start && date <= End;
    /// <summary>Returns whether another range overlaps this range.</summary>
    public bool Overlaps(DateRange other) => Start <= other.End && End >= other.Start;

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Start;
        yield return End;
    }

    /// <inheritdoc />
    public override string ToString() => $"{Start:d} - {End:d}";
}
