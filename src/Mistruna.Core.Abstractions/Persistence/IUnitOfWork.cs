namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Coordinates persistence operations and transactions.</summary>
public interface IUnitOfWork : IDisposable, IAsyncDisposable
{
    /// <summary>Saves pending changes.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    /// <summary>Begins a transaction.</summary>
    Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);
    /// <summary>Executes an action in a transaction.</summary>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default);
    /// <summary>Executes a result-producing action in a transaction.</summary>
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default);
}

/// <summary>Represents a transaction owned by a unit of work.</summary>
public interface IUnitOfWorkTransaction : IDisposable, IAsyncDisposable
{
    /// <summary>Gets the transaction identifier.</summary>
    Guid TransactionId { get; }
    /// <summary>Commits the transaction.</summary>
    Task CommitAsync(CancellationToken cancellationToken = default);
    /// <summary>Rolls back the transaction.</summary>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}

/// <summary>Compatibility alias for a unit-of-work transaction.</summary>
public interface IDbTransaction : IUnitOfWorkTransaction;
