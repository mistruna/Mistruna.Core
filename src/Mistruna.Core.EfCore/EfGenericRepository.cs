using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Mistruna.Core.Abstractions.Persistence;

namespace Mistruna.Core.EfCore;

/// <summary>EF Core implementation of the generic repository contract.</summary>
public class EfGenericRepository<T>(DbContext context) : IGenericRepository<T>
    where T : class, IEntity
{
    private DbSet<T> DbSet { get; } = context.Set<T>();

    /// <inheritdoc />
    public async ValueTask AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);

    /// <inheritdoc />
    public async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
        => await DbSet.AddRangeAsync(entities, cancellationToken);

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
        => await SpecificationEvaluator
            .GetQuery(DbSet.AsQueryable(), specification)
            .ToListAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<T?> FindOneAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
        => await SpecificationEvaluator
            .GetQuery(DbSet.AsQueryable(), specification)
            .FirstOrDefaultAsync(cancellationToken);

    /// <inheritdoc />
    public async Task<int> CountAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default)
        => await SpecificationEvaluator
            .GetQuery(DbSet.AsQueryable(), specification)
            .CountAsync(cancellationToken);

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
