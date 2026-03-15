namespace Mistruna.Core.Contracts.Base.Entities;

/// <summary>
/// Base class for value objects. Value objects are immutable and compared by their values.
/// </summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>
    /// Gets the atomic values that make up this value object.
    /// </summary>
    /// <returns>An enumerable of the atomic values.</returns>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc />
    public bool Equals(ValueObject? other)
    {
        return Equals((object?)other);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (ValueObject)obj;

        return GetEqualityComponents()
            .SequenceEqual(other.GetEqualityComponents());
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(ValueObject? left, ValueObject? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(ValueObject? left, ValueObject? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// A value object that wraps a single value.
/// </summary>
/// <typeparam name="T">The type of the value.</typeparam>
public abstract class SingleValueObject<T> : ValueObject, IComparable<SingleValueObject<T>>
    where T : IComparable<T>
{
    /// <summary>
    /// Gets the wrapped value.
    /// </summary>
    public T Value { get; }

    /// <summary>
    /// Initializes a new instance.
    /// </summary>
    /// <param name="value">The value to wrap.</param>
    protected SingleValueObject(T value)
    {
        Value = value;
    }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public int CompareTo(SingleValueObject<T>? other)
    {
        return other is null ? 1 : Value.CompareTo(other.Value);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    /// <summary>
    /// Implicit conversion to the wrapped type.
    /// </summary>
    /// <param name="valueObject">The value object.</param>
    public static implicit operator T(SingleValueObject<T> valueObject)
    {
        return valueObject.Value;
    }
}
