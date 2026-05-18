namespace Mistruna.Core.Contracts.Base.Responses;

/// <summary>
/// Convenience methods for populating paginated responses.
/// </summary>
public static class PageViewResponseExtensions
{
    /// <summary>
    /// Populates a paginated response with message, metadata and elements.
    /// </summary>
    public static TResponse WithPage<TResponse, TModel>(
        this TResponse response,
        string? message,
        IList<TModel> elements,
        int total,
        int page,
        int count)
        where TResponse : PageViewResponse<TModel>
        where TModel : class
    {
        response.Message = message;
        response.Page = page;
        response.Count = count;
        response.Total = total;
        response.Elements = elements;

        return response;
    }

    /// <summary>
    /// Populates a paginated response using the element count as the page size and total.
    /// </summary>
    public static TResponse WithPage<TResponse, TModel>(
        this TResponse response,
        string? message,
        IList<TModel> elements,
        int page = 1)
        where TResponse : PageViewResponse<TModel>
        where TModel : class
        => response.WithPage(message, elements, elements.Count, page, elements.Count);
}
