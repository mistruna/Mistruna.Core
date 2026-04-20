namespace Mistruna.Core.Payments.Stripe.Events;

public sealed record StripeSubscriptionDeleted(
    string Id,
    DateTimeOffset Created,
    string StripeCustomerId,
    string StripeSubscriptionId) : StripeEvent(Id, "customer.subscription.deleted", Created);
