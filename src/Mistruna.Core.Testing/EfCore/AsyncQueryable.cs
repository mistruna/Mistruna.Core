using System.Collections;
using System.Linq.Expressions;

namespace Mistruna.Core.Testing.EfCore;

/// <summary>
/// Creates in-memory queryables backed by <see cref="TestAsyncQueryProvider{T}"/> for EF Core async tests.
/// </summary>
public static class AsyncQueryable
{
    /// <summary>
    /// Wraps an in-memory sequence as an <see cref="IQueryable{T}"/> that supports EF Core async operators.
    /// </summary>
    public static IQueryable<T> From<T>(IEnumerable<T> source) =>
        new TestAsyncQueryable<T>(source.AsQueryable());
}

internal sealed class TestAsyncQueryable<T> : IQueryable<T>, IAsyncEnumerable<T>
{
    private readonly IQueryable<T> _inner;

    public TestAsyncQueryable(IQueryable<T> inner)
    {
        _inner = inner;
        Provider = new TestAsyncQueryProvider<T>(inner.Provider);
    }

    public Type ElementType => _inner.ElementType;

    public Expression Expression => _inner.Expression;

    public IQueryProvider Provider { get; }

    public IEnumerator<T> GetEnumerator() => _inner.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) =>
        new TestAsyncEnumerator<T>(_inner.GetEnumerator());
}
