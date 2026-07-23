namespace Mistruna.Core.Abstractions.Entities;

/// <summary>Represents an entity with an identifier.</summary>
public interface IEntity<TId> where TId : IEquatable<TId>
{
    /// <summary>Gets the identifier.</summary>
    TId Id { get; }
}

/// <summary>Represents audit metadata.</summary>
public interface IAuditableEntity
{
    /// <summary>Gets or sets the creation time.</summary>
    DateTime CreatedAt { get; set; }
    /// <summary>Gets or sets the creator.</summary>
    string? CreatedBy { get; set; }
    /// <summary>Gets or sets the modification time.</summary>
    DateTime? ModifiedAt { get; set; }
    /// <summary>Gets or sets the modifier.</summary>
    string? ModifiedBy { get; set; }
}

/// <summary>Represents soft-deletion metadata.</summary>
public interface ISoftDeletable
{
    /// <summary>Gets or sets whether the entity is deleted.</summary>
    bool IsDeleted { get; set; }
    /// <summary>Gets or sets the deletion time.</summary>
    DateTime? DeletedAt { get; set; }
    /// <summary>Gets or sets who deleted the entity.</summary>
    string? DeletedBy { get; set; }
}

/// <summary>Base entity identified by <typeparamref name="TId"/>.</summary>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
    where TId : IEquatable<TId>
{
    /// <inheritdoc />
    public virtual TId Id { get; protected set; } = default!;

    /// <inheritdoc />
    public bool Equals(Entity<TId>? other)
    {
        if (other is null)
            return false;
        if (ReferenceEquals(this, other))
            return true;
        if (GetType() != other.GetType())
            return false;
        if (Id.Equals(default!) || other.Id.Equals(default!))
            return false;
        return Id.Equals(other.Id);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is Entity<TId> entity && Equals(entity);

    /// <inheritdoc />
    public override int GetHashCode() => (GetType().GetHashCode() * 907) + Id.GetHashCode();

    /// <summary>Compares entities for equality.</summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right) =>
        left is null ? right is null : left.Equals(right);

    /// <summary>Compares entities for inequality.</summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right) => !(left == right);
}

/// <summary>Base entity identified by a GUID.</summary>
public abstract class Entity : Entity<Guid>
{
    /// <summary>Initializes an entity with a new GUID.</summary>
    protected Entity() => Id = Guid.NewGuid();
    /// <summary>Initializes an entity with a GUID.</summary>
    protected Entity(Guid id) => Id = id;
}

/// <summary>Base auditable entity.</summary>
public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity where TId : IEquatable<TId>
{
    /// <inheritdoc />
    public DateTime CreatedAt { get; set; }
    /// <inheritdoc />
    public string? CreatedBy { get; set; }
    /// <inheritdoc />
    public DateTime? ModifiedAt { get; set; }
    /// <inheritdoc />
    public string? ModifiedBy { get; set; }
}

/// <summary>Base auditable GUID entity.</summary>
public abstract class AuditableEntity : AuditableEntity<Guid>
{
    /// <summary>Initializes an entity with a new GUID.</summary>
    protected AuditableEntity() => Id = Guid.NewGuid();
    /// <summary>Initializes an entity with a GUID.</summary>
    protected AuditableEntity(Guid id) => Id = id;
}

/// <summary>Base soft-deletable entity.</summary>
public abstract class SoftDeletableEntity<TId> : AuditableEntity<TId>, ISoftDeletable
    where TId : IEquatable<TId>
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }
    /// <inheritdoc />
    public DateTime? DeletedAt { get; set; }
    /// <inheritdoc />
    public string? DeletedBy { get; set; }

    /// <summary>Marks the entity as deleted.</summary>
    public virtual void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>Restores the entity.</summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}

/// <summary>Base soft-deletable GUID entity.</summary>
public abstract class SoftDeletableEntity : SoftDeletableEntity<Guid>
{
    /// <summary>Initializes an entity with a new GUID.</summary>
    protected SoftDeletableEntity() => Id = Guid.NewGuid();
    /// <summary>Initializes an entity with a GUID.</summary>
    protected SoftDeletableEntity(Guid id) => Id = id;
}
