namespace Mistruna.Core.Payments.Stripe.Events;

public sealed record StripeInvoicePaymentFailed(
    string Id,
    DateTimeOffset Created,
    string StripeCustomerId,
    string StripeInvoiceId,
    long AmountDue,
    string Currency) : StripeEvent(Id, "invoice.payment_failed", Created);
