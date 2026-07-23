namespace Mistruna.Core.Abstractions.Responses;

/// <summary>Represents a response containing one item.</summary>
public class ItemResponse<T> : IResponse where T : class
{
    /// <inheritdoc />
    public string? Message { get; set; }
    /// <summary>Gets or sets the returned item.</summary>
    public T? Item { get; set; }
}
