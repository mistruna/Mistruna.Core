namespace Mistruna.Core.Base.Responses;

/// <summary>
/// Represents a paginated view of items with navigation metadata.
/// </summary>
/// <typeparam name="TModel">The type of items in the page.</typeparam>
public class PageView<TModel> where TModel : class
{
    /// <summary>Current page number (1-based).</summary>
    public int Page { get; set; }

    /// <summary>Number of items per page.</summary>
    public int Count { get; set; }

    /// <summary>Total number of items across all pages.</summary>
    public int Total { get; set; }

    /// <summary>List of items for the current page.</summary>
    public IList<TModel> Elements { get; set; } = new List<TModel>();

    /// <summary>Gets the total number of pages.</summary>
    public int TotalPages => Count > 0 ? (int)Math.Ceiling((double)Total / Count) : 0;

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => Page > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => Page < TotalPages;

    /// <summary>Gets the first item number on the current page (1-based).</summary>
    public int FirstItemOnPage => Total > 0 ? (Page - 1) * Count + 1 : 0;

    /// <summary>Gets the last item number on the current page (1-based).</summary>
    public int LastItemOnPage => Total > 0 ? Math.Min(Page * Count, Total) : 0;

    /// <summary>
    /// Creates an empty PageView.
    /// </summary>
    public static PageView<TModel> Empty(int page = 1, int count = 10) => new()
    {
        Page = page,
        Count = count,
        Total = 0,
        Elements = new List<TModel>()
    };

    /// <summary>
    /// Creates a PageView from a collection with pagination parameters.
    /// </summary>
    /// <param name="items">The items for the current page.</param>
    /// <param name="totalCount">The total count of all items.</param>
    /// <param name="page">The current page number.</param>
    /// <param name="pageSize">The page size.</param>
    public static PageView<TModel> Create(
        IList<TModel> items,
        int totalCount,
        int page,
        int pageSize) => new()
    {
        Page = page,
        Count = pageSize,
        Total = totalCount,
        Elements = items
    };
}
