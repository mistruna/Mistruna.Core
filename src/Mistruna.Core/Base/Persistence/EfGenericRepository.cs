using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Base.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IGenericRepository{T}"/>.
/// Provides full CRUD, specification-based queries, and paging out of the box.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
/// <typeparam name="TContext">The DbContext type.</typeparam>
public abstract class EfGenericRepository<T, TContext>(TContext context, DbSet<T> dbSet)
    : IGenericRepository<T>
    where T : class, IEntity
    where TContext : DbContext
{
    protected TContext Context { get; } = context;
    protected DbSet<T> DbSet { get; } = dbSet;

    /// <inheritdoc />
    public async ValueTask AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await DbSet.FindAsync([id], cancellationToken);

    /// <inheritdoc />
    public async ValueTask<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await DbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<IReadOnlyList<T>> FindAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
        return await query.ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<T?> FindOneAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
    {
        var query = SpecificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
        return await query.CountAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
        => predicate is null
            ? await DbSet.CountAsync(cancellationToken)
            : await DbSet.CountAsync(predicate, cancellationToken);

    /// <inheritdoc />
    public async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
        => await DbSet.AnyAsync(predicate, cancellationToken);

    /// <inheritdoc />
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task UpdateRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task DeleteRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public IQueryable<T> AsQueryable() => DbSet.AsNoTracking();
}
