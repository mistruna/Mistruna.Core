using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Base.Responses;

/// <summary>
/// Standard response for single item operations (Get/Create/Update).
/// </summary>
/// <typeparam name="T">Type of the returned item.</typeparam>
public class ItemResponse<T> : IResponse where T : class
{
    /// <summary>Operation message (success, warning, error).</summary>
    public string? Message { get; set; }

    /// <summary>Returned item (can be null).</summary>
    public T? Item { get; set; }
}
