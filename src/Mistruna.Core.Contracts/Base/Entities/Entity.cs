namespace Mistruna.Core.Contracts.Base.Entities;

/// <summary>
/// Interface for entities with a unique identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public interface IEntity<TId> where TId : IEquatable<TId>
{
    /// <summary>
    /// Gets the unique identifier.
    /// </summary>
    TId Id { get; }
}

/// <summary>
/// Interface for entities with audit information.
/// </summary>
public interface IAuditableEntity
{
    /// <summary>
    /// Gets or sets the creation timestamp.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets who created the entity.
    /// </summary>
    string? CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets the last modification timestamp.
    /// </summary>
    DateTime? ModifiedAt { get; set; }

    /// <summary>
    /// Gets or sets who last modified the entity.
    /// </summary>
    string? ModifiedBy { get; set; }
}

/// <summary>
/// Interface for soft-deletable entities.
/// </summary>
public interface ISoftDeletable
{
    /// <summary>
    /// Gets or sets whether the entity is deleted.
    /// </summary>
    bool IsDeleted { get; set; }

    /// <summary>
    /// Gets or sets the deletion timestamp.
    /// </summary>
    DateTime? DeletedAt { get; set; }

    /// <summary>
    /// Gets or sets who deleted the entity.
    /// </summary>
    string? DeletedBy { get; set; }
}

/// <summary>
/// Base class for entities with a unique identifier.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class Entity<TId> : IEntity<TId>, IEquatable<Entity<TId>>
    where TId : IEquatable<TId>
{
    /// <inheritdoc />
    public virtual TId Id { get; protected set; } = default!;

    /// <summary>
    /// Determines whether the specified entity is equal to the current entity.
    /// </summary>
    /// <param name="other">The entity to compare with.</param>
    /// <returns>True if equal, otherwise false.</returns>
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
    public override bool Equals(object? obj)
    {
        return obj is Entity<TId> entity && Equals(entity);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return (GetType().GetHashCode() * 907) + Id.GetHashCode();
    }

    /// <summary>
    /// Equality operator.
    /// </summary>
    public static bool operator ==(Entity<TId>? left, Entity<TId>? right)
    {
        if (left is null && right is null)
            return true;

        if (left is null || right is null)
            return false;

        return left.Equals(right);
    }

    /// <summary>
    /// Inequality operator.
    /// </summary>
    public static bool operator !=(Entity<TId>? left, Entity<TId>? right)
    {
        return !(left == right);
    }
}

/// <summary>
/// Base class for entities with a GUID identifier.
/// </summary>
public abstract class Entity : Entity<Guid>
{
    /// <summary>
    /// Initializes a new instance with a new GUID.
    /// </summary>
    protected Entity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance with the specified GUID.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    protected Entity(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Base class for auditable entities.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class AuditableEntity<TId> : Entity<TId>, IAuditableEntity
    where TId : IEquatable<TId>
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

/// <summary>
/// Base class for auditable entities with a GUID identifier.
/// </summary>
public abstract class AuditableEntity : AuditableEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance with a new GUID.
    /// </summary>
    protected AuditableEntity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance with the specified GUID.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    protected AuditableEntity(Guid id)
    {
        Id = id;
    }
}

/// <summary>
/// Base class for soft-deletable auditable entities.
/// </summary>
/// <typeparam name="TId">The type of the identifier.</typeparam>
public abstract class SoftDeletableEntity<TId> : AuditableEntity<TId>, ISoftDeletable
    where TId : IEquatable<TId>
{
    /// <inheritdoc />
    public bool IsDeleted { get; set; }

    /// <inheritdoc />
    public DateTime? DeletedAt { get; set; }

    /// <inheritdoc />
    public string? DeletedBy { get; set; }

    /// <summary>
    /// Marks the entity as deleted.
    /// </summary>
    /// <param name="deletedBy">Who is deleting the entity.</param>
    public virtual void Delete(string? deletedBy = null)
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        DeletedBy = deletedBy;
    }

    /// <summary>
    /// Restores the entity (undeletes it).
    /// </summary>
    public virtual void Restore()
    {
        IsDeleted = false;
        DeletedAt = null;
        DeletedBy = null;
    }
}

/// <summary>
/// Base class for soft-deletable auditable entities with a GUID identifier.
/// </summary>
public abstract class SoftDeletableEntity : SoftDeletableEntity<Guid>
{
    /// <summary>
    /// Initializes a new instance with a new GUID.
    /// </summary>
    protected SoftDeletableEntity()
    {
        Id = Guid.NewGuid();
    }

    /// <summary>
    /// Initializes a new instance with the specified GUID.
    /// </summary>
    /// <param name="id">The unique identifier.</param>
    protected SoftDeletableEntity(Guid id)
    {
        Id = id;
    }
}
