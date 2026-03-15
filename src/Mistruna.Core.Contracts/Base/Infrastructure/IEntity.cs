namespace Mistruna.Core.Contracts.Base.Infrastructure;

/// <summary>
/// Represents the base entity with audit timestamps.
/// </summary>
public interface IEntity
{
    /// <summary>Entity identifier.</summary>
    Guid Id { get; set; }

    /// <summary>Date when the entity was created (UTC).</summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>Date when the entity was last modified (UTC).</summary>
    DateTimeOffset? UpdatedAt { get; set; }
}
