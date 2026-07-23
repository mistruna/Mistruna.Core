namespace Mistruna.Core.Abstractions.Responses;

/// <summary>Represents a deletion response.</summary>
public class DeleteResponse : IResponse
{
    /// <inheritdoc />
    public string? Message { get; set; }
    /// <summary>Gets or sets the deleted identifier.</summary>
    public Guid Id { get; set; }
}
