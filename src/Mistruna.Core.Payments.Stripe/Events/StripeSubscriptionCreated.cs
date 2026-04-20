namespace Mistruna.Core.Payments.Stripe.Events;

public sealed record StripeSubscriptionCreated(
    string Id,
    DateTimeOffset Created,
    string StripeCustomerId,
    string StripeSubscriptionId,
    string StripePriceId,
    string Status,
    DateTimeOffset CurrentPeriodStart,
    DateTimeOffset CurrentPeriodEnd) : StripeEvent(Id, "customer.subscription.created", Created);
