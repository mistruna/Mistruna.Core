namespace Mistruna.Core.Payments.Stripe.Events;

/// <summary>
/// Catch-all for event types we haven't modeled. <see cref="RawJson"/> is the full event
/// payload so the outbox retains enough context to be reprocessed later if we add support.
/// </summary>
public sealed record StripeUnknownEvent(
    string Id,
    string Type,
    DateTimeOffset Created,
    string RawJson) : StripeEvent(Id, Type, Created);
