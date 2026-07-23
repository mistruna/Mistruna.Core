namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Represents an entity with user audit metadata.</summary>
public interface IAuditable : IEntity
{
    /// <summary>Gets or sets the creator identifier.</summary>
    Guid CreatedBy { get; set; }
    /// <summary>Gets or sets the modifier identifier.</summary>
    Guid? ModifiedBy { get; set; }
}
