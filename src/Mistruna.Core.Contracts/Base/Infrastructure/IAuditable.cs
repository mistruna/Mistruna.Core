namespace Mistruna.Core.Contracts.Base.Infrastructure;

/// <summary>
/// Represents an auditable entity with user tracking.
/// </summary>
public interface IAuditable : IEntity
{
    /// <summary>Identifier of the user who created the entity.</summary>
    Guid CreatedBy { get; set; }

    /// <summary>Identifier of the user who last modified the entity.</summary>
    Guid? ModifiedBy { get; set; }
}
