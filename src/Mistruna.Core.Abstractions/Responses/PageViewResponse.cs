namespace Mistruna.Core.Abstractions.Responses;

/// <summary>Represents a paginated response.</summary>
public class PageViewResponse<T> : PageView<T>, IResponse where T : class
{
    /// <inheritdoc />
    public string? Message { get; set; }
}
