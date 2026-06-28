using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;

namespace Mistruna.Core.Providers;

/// <summary>
/// In-memory async query provider for unit tests that exercise EF Core repositories
/// without a real database.
/// </summary>
/// <typeparam name="T">The entity type exposed by the queryable.</typeparam>
public class TestAsyncQueryProvider<T>(IQueryProvider inner) : IAsyncQueryProvider
{
    /// <inheritdoc />
    public IQueryable CreateQuery(Expression expression) => new TestAsyncEnumerable<T>(expression);

    /// <inheritdoc />
    public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        => new TestAsyncEnumerable<TElement>(expression);

    /// <inheritdoc />
    public object? Execute(Expression expression) => inner.Execute(expression);

    /// <inheritdoc />
    public TResult Execute<TResult>(Expression expression) => inner.Execute<TResult>(expression);

    /// <inheritdoc />
    public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
    {
        var expectedResultType = typeof(TResult).GetGenericArguments()[0];
        var executionResult = typeof(IQueryProvider)
            .GetMethod(nameof(IQueryProvider.Execute), 1, [typeof(Expression)])
            ?.MakeGenericMethod(expectedResultType)
            .Invoke(this, [expression]);

        return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))!
            .MakeGenericMethod(expectedResultType)
            .Invoke(null, [executionResult])!;
    }
}
