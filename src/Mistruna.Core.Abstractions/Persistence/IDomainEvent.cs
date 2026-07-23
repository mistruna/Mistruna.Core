namespace Mistruna.Core.Abstractions.Persistence;

/// <summary>Marks a domain event.</summary>
public interface IDomainEvent
{
    /// <summary>Gets when the event occurred.</summary>
    DateTime OccurredAt { get; }
}

/// <summary>Handles a domain event.</summary>
public interface IDomainEventHandler<in TEvent> where TEvent : IDomainEvent
{
    /// <summary>Handles the event.</summary>
    Task HandleAsync(TEvent domainEvent, CancellationToken cancellationToken = default);
}

/// <summary>Dispatches domain events.</summary>
public interface IDomainEventDispatcher
{
    /// <summary>Dispatches an event to registered handlers.</summary>
    Task DispatchAsync<TEvent>(TEvent domainEvent, CancellationToken cancellationToken = default)
        where TEvent : IDomainEvent;
}

/// <summary>Base persistence entity that raises domain events.</summary>
public abstract class AggregateRoot : IEntity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    /// <inheritdoc />
    public Guid Id { get; set; }
    /// <inheritdoc />
    public DateTimeOffset CreatedAt { get; set; }
    /// <inheritdoc />
    public DateTimeOffset? UpdatedAt { get; set; }

    /// <summary>Gets pending domain events.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Adds a domain event.</summary>
    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
    /// <summary>Removes a domain event.</summary>
    protected void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents.Remove(domainEvent);
    /// <summary>Clears pending domain events.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
