namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Represents an entity with audit timestamps.</summary>
public interface IEntity
{
    /// <summary>Gets or sets the identifier.</summary>
    Guid Id { get; set; }
    /// <summary>Gets or sets the creation time.</summary>
    DateTimeOffset CreatedAt { get; set; }
    /// <summary>Gets or sets the last update time.</summary>
    DateTimeOffset? UpdatedAt { get; set; }
}
