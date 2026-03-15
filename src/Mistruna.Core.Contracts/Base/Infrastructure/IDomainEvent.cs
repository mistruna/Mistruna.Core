namespace Mistruna.Core.Contracts.Base.Infrastructure;

/// <summary>
/// Marker interface for domain events.
/// </summary>
/// <remarks>
/// Domain events represent something important that happened in the domain.
/// They are used to notify other parts of the system about changes.
/// </remarks>
/// <example>
/// <code>
/// public class UserCreatedEvent : IDomainEvent
/// {
///     public Guid UserId { get; init; }
///     public string Email { get; init; }
///     public DateTime OccurredAt { get; } = DateTime.UtcNow;
/// }
/// </code>
/// </example>
public interface IDomainEvent
{
    /// <summary>
    /// Gets the date and time when the event occurred.
    /// </summary>
    DateTime OccurredAt { get; }
}

/// <summary>
/// Interface for handling domain events.
/// </summary>
/// <typeparam name="TEvent">The type of domain event.</typeparam>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>
    /// Handles the domain event.
    /// </summary>
    /// <param name="domainEvent">The event to handle.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    System.Threading.Tasks.Task HandleAsync(TEvent domainEvent, System.Threading.CancellationToken cancellationToken = default);
}

/// <summary>
/// Interface for publishing domain events.
/// </summary>
public interface IDomainEventDispatcher
{
    /// <summary>
    /// Dispatches a domain event to all registered handlers.
    /// </summary>
    /// <typeparam name="TEvent">The type of event.</typeparam>
    /// <param name="domainEvent">The event to dispatch.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    System.Threading.Tasks.Task DispatchAsync<TEvent>(TEvent domainEvent, System.Threading.CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}

/// <summary>
/// Base class for domain entities that raise events.
/// </summary>
public abstract class AggregateRoot : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    #region IEntity

    /// <inheritdoc />
    public Guid Id { get; set; }

    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }

    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    #endregion

    /// <summary>
    /// Gets the domain events that have been raised.
    /// </summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>
    /// Adds a domain event to be published.
    /// </summary>
    /// <param name="domainEvent">The domain event.</param>
    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    /// <summary>
    /// Removes a domain event.
    /// </summary>
    /// <param name="domainEvent">The domain event to remove.</param>
    protected void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Remove(domainEvent);
    }

    /// <summary>
    /// Clears all domain events.
    /// </summary>
    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }
}
