namespace Mistruna.Core.Testing.EfCore;

/// <summary>
/// Adapts a synchronous enumerator to <see cref="IAsyncEnumerator{T}"/> for repository unit tests.
/// </summary>
/// <typeparam name="T">The element type.</typeparam>
public class TestAsyncEnumerator<T>(IEnumerator<T> inner) : IAsyncEnumerator<T>
{
    /// <inheritdoc />
    public T Current => inner.Current;

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        inner.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask<bool> MoveNextAsync() => new(inner.MoveNext());
}
