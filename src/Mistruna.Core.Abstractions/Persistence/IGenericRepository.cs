using System.Linq.Expressions;

namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Read-only repository contract.</summary>
public interface IReadRepository<T> where T : class, IEntity
{
    /// <summary>Gets an entity by identifier.</summary>
    ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    /// <summary>Gets all entities.</summary>
    ValueTask<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    /// <summary>Finds entities using a predicate.</summary>
    Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <summary>Finds entities using a specification.</summary>
    Task<IReadOnlyList<T>> FindAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    /// <summary>Finds one entity using a specification.</summary>
    Task<T?> FindOneAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    /// <summary>Counts entities using a specification.</summary>
    Task<int> CountAsync(ISpecification<T> specification, CancellationToken cancellationToken = default);
    /// <summary>Counts entities using an optional predicate.</summary>
    Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
    /// <summary>Checks whether any entity matches.</summary>
    Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
    /// <summary>Returns an advanced query surface.</summary>
    IQueryable<T> AsQueryable();
}

/// <summary>Write-only repository contract.</summary>
public interface IWriteRepository<T> where T : class, IEntity
{
    /// <summary>Adds an entity to the current persistence context.</summary>
    ValueTask AddAsync(T entity, CancellationToken cancellationToken = default);
    /// <summary>Adds multiple entities.</summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    /// <summary>Updates an entity.</summary>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    /// <summary>Updates multiple entities.</summary>
    Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    /// <summary>Deletes an entity.</summary>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
    /// <summary>Deletes multiple entities.</summary>
    Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
}

/// <summary>Combined read/write repository contract.</summary>
public interface IGenericRepository<T> : IReadRepository<T>, IWriteRepository<T> where T : class, IEntity;
