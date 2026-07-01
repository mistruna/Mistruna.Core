using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mistruna.Core.Contracts.Base.Infrastructure;

namespace Mistruna.Core.Base.Persistence;

/// <summary>
/// EF Core implementation of <see cref="IUnitOfWork"/>.
/// Wraps <see cref="DbContext"/> to provide transactional boundaries.
/// </summary>
/// <typeparam name="TContext">The DbContext type.</typeparam>
public sealed class EfUnitOfWork<TContext>(TContext context) : IUnitOfWork
    where TContext : DbContext
{
    private IDbContextTransaction? _currentTransaction;
    private bool _disposed;

    /// <inheritdoc />
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        return await context.SaveChangesAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IUnitOfWorkTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();

        if (_currentTransaction is not null)
        {
            throw new InvalidOperationException("A database transaction is already active for this unit of work.");
        }

        _currentTransaction = await context.Database.BeginTransactionAsync(cancellationToken);
        return new EfDbTransaction(_currentTransaction, ClearTransaction);
    }

    /// <inheritdoc />
    public async Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var strategy = context.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async operationCancellationToken =>
        {
            await using var transaction = await BeginTransactionAsync(operationCancellationToken);
            try
            {
                await action(operationCancellationToken);
                await SaveChangesAsync(operationCancellationToken);
                await transaction.CommitAsync(operationCancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(action);

        var strategy = context.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async operationCancellationToken =>
        {
            await using var transaction = await BeginTransactionAsync(operationCancellationToken);
            try
            {
                var result = await action(operationCancellationToken);
                await SaveChangesAsync(operationCancellationToken);
                await transaction.CommitAsync(operationCancellationToken);
                return result;
            }
            catch
            {
                await transaction.RollbackAsync(CancellationToken.None);
                throw;
            }
        }, cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _currentTransaction?.Dispose();
        context.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        if (_currentTransaction is not null)
        {
            await _currentTransaction.DisposeAsync();
        }

        await context.DisposeAsync();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private void ClearTransaction(IDbContextTransaction transaction)
    {
        if (ReferenceEquals(_currentTransaction, transaction))
        {
            _currentTransaction = null;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(EfUnitOfWork<TContext>));
        }
    }
}

/// <summary>
/// Wraps EF Core's <see cref="IDbContextTransaction"/> to implement <see cref="IUnitOfWorkTransaction"/>.
/// </summary>
internal sealed class EfDbTransaction(
    IDbContextTransaction transaction,
    Action<IDbContextTransaction> onCompleted) : IUnitOfWorkTransaction, IDbTransaction
{
    private bool _completed;
    private bool _disposed;

    /// <inheritdoc />
    public Guid TransactionId { get; } = transaction.TransactionId;

    /// <inheritdoc />
    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_completed)
        {
            return;
        }

        await transaction.CommitAsync(cancellationToken);
        Complete();
    }

    /// <inheritdoc />
    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        if (_completed)
        {
            return;
        }

        await transaction.RollbackAsync(cancellationToken);
        Complete();
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            if (!_completed)
            {
                transaction.Rollback();
            }
        }
        finally
        {
            transaction.Dispose();
            _disposed = true;
            Complete();
            GC.SuppressFinalize(this);
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
        {
            return;
        }

        try
        {
            if (!_completed)
            {
                await transaction.RollbackAsync();
            }
        }
        finally
        {
            await transaction.DisposeAsync();
            _disposed = true;
            Complete();
            GC.SuppressFinalize(this);
        }
    }

    private void Complete()
    {
        if (_completed)
        {
            return;
        }

        _completed = true;
        onCompleted(transaction);
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(EfDbTransaction));
        }
    }
}
