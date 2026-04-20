namespace Mistruna.Core.Payments.Stripe.Events;

/// <summary>
/// Base type for parsed Stripe webhook events. Every known event is represented by a derived
/// record. Unrecognized events are projected as <see cref="StripeUnknownEvent"/> so the outbox
/// can still persist them without deserialization failures.
/// </summary>
public abstract record StripeEvent(string Id, string Type, DateTimeOffset Created);
