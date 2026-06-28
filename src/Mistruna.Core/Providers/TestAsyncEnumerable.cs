using System.Linq.Expressions;

namespace Mistruna.Core.Providers;

/// <summary>
/// Wraps an in-memory <see cref="EnumerableQuery{T}"/> as <see cref="IAsyncEnumerable{T}"/>
/// for EF Core async LINQ tests.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>
{
    /// <summary>
    /// Initializes a new instance from an in-memory sequence.
    /// </summary>
    public TestAsyncEnumerable(IEnumerable<T> enumerable) : base(enumerable)
    {
    }

    /// <summary>
    /// Initializes a new instance from a LINQ expression tree.
    /// </summary>
    public TestAsyncEnumerable(Expression expression) : base(expression)
    {
    }

    /// <inheritdoc />
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
}
