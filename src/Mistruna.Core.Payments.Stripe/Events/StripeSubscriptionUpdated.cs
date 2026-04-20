namespace Mistruna.Core.Payments.Stripe.Events;

public sealed record StripeSubscriptionUpdated(
    string Id,
    DateTimeOffset Created,
    string StripeCustomerId,
    string StripeSubscriptionId,
    string StripePriceId,
    string Status,
    DateTimeOffset CurrentPeriodStart,
    DateTimeOffset CurrentPeriodEnd,
    DateTimeOffset? CancelAt) : StripeEvent(Id, "customer.subscription.updated", Created);
