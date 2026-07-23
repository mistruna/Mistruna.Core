namespace Mistruna.Core.Abstractions.Responses;

/// <summary>Represents a response with an optional message.</summary>
public interface IResponse
{
    /// <summary>Gets or sets the response message.</summary>
    string? Message { get; set; }
}
