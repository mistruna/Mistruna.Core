namespace Mistruna.Core.Extensions;

/// <summary>
/// Provides extension methods for working with collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Checks if the collection is null or empty.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to check.</param>
    /// <returns>True if the collection is null or empty.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
        => source is null || !source.Any();

    /// <summary>
    /// Checks if the collection has any elements.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to check.</param>
    /// <returns>True if the collection has elements.</returns>
    public static bool HasItems<T>(this IEnumerable<T>? source)
        => source is not null && source.Any();

    /// <summary>
    /// Returns the collection or an empty collection if null.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <returns>The collection or an empty enumerable.</returns>
    public static IEnumerable<T> OrEmpty<T>(this IEnumerable<T>? source)
        => source ?? Enumerable.Empty<T>();

    /// <summary>
    /// Performs an action on each element of the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="action">The action to perform.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }

    /// <summary>
    /// Performs an action on each element with its index.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="action">The action to perform with index.</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        var index = 0;
        foreach (var item in source)
            action(item, index++);
    }

    /// <summary>
    /// Splits a collection into batches of a specified size.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection to split.</param>
    /// <param name="batchSize">The size of each batch.</param>
    /// <returns>An enumerable of batches.</returns>
    public static IEnumerable<List<T>> Batch<T>(this IEnumerable<T> source, int batchSize)
    {
        var batch = new List<T>(batchSize);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == batchSize)
            {
                yield return batch;
                batch = new List<T>(batchSize);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

#if NETSTANDARD2_0
    /// <summary>
    /// Removes duplicates based on a key selector.
    /// Available in .NET 6+ as System.Linq.Enumerable.DistinctBy
    /// </summary>
    /// <typeparam name="TSource">The type of elements in the collection.</typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="keySelector">The key selector function.</param>
    /// <returns>A collection with duplicates removed.</returns>
    public static IEnumerable<TSource> DistinctBy<TSource, TKey>(
        this IEnumerable<TSource> source,
        Func<TSource, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();
        foreach (var element in source)
        {
            if (seen.Add(keySelector(element)))
                yield return element;
        }
    }
#endif

    /// <summary>
    /// Converts a collection to a HashSet.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <returns>A new HashSet containing the elements.</returns>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        => new(source);

    /// <summary>
    /// Converts a collection to a HashSet with a custom comparer.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="comparer">The equality comparer.</param>
    /// <returns>A new HashSet containing the elements.</returns>
    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source, IEqualityComparer<T> comparer)
        => new(source, comparer);

    /// <summary>
    /// Filters out null values from a collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <returns>A collection without null values.</returns>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        => source.Where(x => x is not null)!;

    /// <summary>
    /// Returns a random element from the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <returns>A random element.</returns>
    public static T? RandomElement<T>(this IList<T> source)
        => source.Count == 0 ? default : source[Random.Shared.Next(source.Count)];

    /// <summary>
    /// Appends an item to the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="item">The item to append.</param>
    /// <returns>The collection with the item appended.</returns>
    public static IEnumerable<T> Append<T>(this IEnumerable<T> source, T item)
    {
        foreach (var element in source)
            yield return element;
        yield return item;
    }

    /// <summary>
    /// Prepends an item to the collection.
    /// </summary>
    /// <typeparam name="T">The type of elements in the collection.</typeparam>
    /// <param name="source">The collection.</param>
    /// <param name="item">The item to prepend.</param>
    /// <returns>The collection with the item prepended.</returns>
    public static IEnumerable<T> Prepend<T>(this IEnumerable<T> source, T item)
    {
        yield return item;
        foreach (var element in source)
            yield return element;
    }
}
