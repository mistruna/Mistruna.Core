namespace Mistruna.Core.Contracts.Base.Infrastructure;

/// <summary>
/// Represents a unit of work for managing database transactions.
/// </summary>
/// <remarks>
/// The Unit of Work pattern maintains a list of objects affected by a business transaction
/// and coordinates the writing out of changes.
/// </remarks>
/// <example>
/// <code>
/// await using var transaction = await unitOfWork.BeginTransactionAsync();
/// try
/// {
///     await userRepository.AddAsync(user);
///     await orderRepository.AddAsync(order);
///     await unitOfWork.SaveChangesAsync();
///     await transaction.CommitAsync();
/// }
/// catch
/// {
///     await transaction.RollbackAsync();
///     throw;
/// }
/// </code>
/// </example>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Saves all changes made in this unit of work to the database.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of entities written to the database.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a new database transaction.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A transaction object that must be committed or rolled back.</returns>
    Task<IDbTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a block of code within a transaction.
    /// </summary>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task ExecuteInTransactionAsync(
        Func<CancellationToken, Task> action,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a block of code within a transaction and returns a result.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    /// <param name="action">The action to execute.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the action.</returns>
    Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<CancellationToken, Task<TResult>> action,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a database transaction.
/// </summary>
public interface IDbTransaction : IDisposable
{
    /// <summary>
    /// Gets the transaction identifier.
    /// </summary>
    Guid TransactionId { get; }

    /// <summary>
    /// Commits the transaction.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task CommitAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the transaction.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task RollbackAsync(CancellationToken cancellationToken = default);
}
