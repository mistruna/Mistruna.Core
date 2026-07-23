namespace Mistruna.Core.Abstractions.Responses;

/// <summary>Represents a page of items and navigation metadata.</summary>
public class PageView<TModel> where TModel : class
{
    /// <summary>Gets or sets the one-based page number.</summary>
    public int Page { get; set; }
    /// <summary>Gets or sets the page size.</summary>
    public int Count { get; set; }
    /// <summary>Gets or sets the total item count.</summary>
    public int Total { get; set; }
    /// <summary>Gets or sets the page items.</summary>
    public IList<TModel> Elements { get; set; } = new List<TModel>();
    /// <summary>Gets the total page count.</summary>
    public int TotalPages => Count > 0 ? (int)Math.Ceiling((double)Total / Count) : 0;
    /// <summary>Gets whether a previous page exists.</summary>
    public bool HasPreviousPage => Page > 1;
    /// <summary>Gets whether a next page exists.</summary>
    public bool HasNextPage => Page < TotalPages;
    /// <summary>Gets the one-based first item number.</summary>
    public int FirstItemOnPage => Total > 0 ? (Page - 1) * Count + 1 : 0;
    /// <summary>Gets the one-based last item number.</summary>
    public int LastItemOnPage => Total > 0 ? Math.Min(Page * Count, Total) : 0;

    /// <summary>Creates an empty page.</summary>
    public static PageView<TModel> Empty(int page = 1, int count = 10) =>
        new() { Page = page, Count = count };

    /// <summary>Creates a populated page.</summary>
    public static PageView<TModel> Create(IList<TModel> items, int totalCount, int page, int pageSize) =>
        new() { Page = page, Count = pageSize, Total = totalCount, Elements = items };
}
