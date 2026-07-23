namespace Mistruna.Core.Abstractions.Responses;

/// <summary>Represents an existence-check response.</summary>
public class ExistResponse : IResponse
{
    /// <inheritdoc />
    public string? Message { get; set; }
    /// <summary>Gets or sets whether the item exists.</summary>
    public bool Exist { get; set; }
}
