namespace Mistruna.Core.Abstractions.Entities;

/// <summary>Base class for immutable, value-compared objects.</summary>
public abstract class ValueObject : IEquatable<ValueObject>
{
    /// <summary>Returns the atomic equality components.</summary>
    protected abstract IEnumerable<object?> GetEqualityComponents();

    /// <inheritdoc />
    public bool Equals(ValueObject? other) => Equals((object?)other);

    /// <inheritdoc />
    public override bool Equals(object? obj) =>
        obj is not null &&
        obj.GetType() == GetType() &&
        GetEqualityComponents().SequenceEqual(((ValueObject)obj).GetEqualityComponents());

    /// <inheritdoc />
    public override int GetHashCode() =>
        GetEqualityComponents().Select(component => component?.GetHashCode() ?? 0).Aggregate((x, y) => x ^ y);

    /// <summary>Compares value objects for equality.</summary>
    public static bool operator ==(ValueObject? left, ValueObject? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Compares value objects for inequality.</summary>
    public static bool operator !=(ValueObject? left, ValueObject? right) => !(left == right);
}

/// <summary>Base class for a value object containing one comparable value.</summary>
public abstract class SingleValueObject<T> : ValueObject, IComparable<SingleValueObject<T>>
    where T : IComparable<T>
{
    /// <summary>Initializes the value object.</summary>
    protected SingleValueObject(T value) => Value = value;

    /// <summary>Gets the wrapped value.</summary>
    public T Value { get; }

    /// <inheritdoc />
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }

    /// <inheritdoc />
    public int CompareTo(SingleValueObject<T>? other) => other is null ? 1 : Value.CompareTo(other.Value);

    /// <inheritdoc />
    public override string ToString() => Value?.ToString() ?? string.Empty;

    /// <summary>Converts to the wrapped value.</summary>
    public static implicit operator T(SingleValueObject<T> valueObject) => valueObject.Value;
}
