using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Contracts.Base.Responses;

/// <summary>Standard response of deletion operation</summary>
public class DeleteResponse : IResponse
{
    /// <summary>Return Message</summary>
    public string? Message { get; set; }

    /// <summary>Id of deleted object</summary>
    public Guid Id { get; set; }
}
