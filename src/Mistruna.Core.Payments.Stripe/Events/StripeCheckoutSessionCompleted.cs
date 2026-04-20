namespace Mistruna.Core.Payments.Stripe.Events;

public sealed record StripeCheckoutSessionCompleted(
    string Id,
    DateTimeOffset Created,
    string StripeCustomerId,
    string StripeSubscriptionId,
    string? ClientReferenceId) : StripeEvent(Id, "checkout.session.completed", Created);
