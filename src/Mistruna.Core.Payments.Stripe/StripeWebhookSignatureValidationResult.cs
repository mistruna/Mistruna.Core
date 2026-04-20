namespace Mistruna.Core.Payments.Stripe;

public sealed class StripeWebhookSignatureValidationResult
{
    private StripeWebhookSignatureValidationResult() { }

    public bool IsValid { get; private init; }
    public string? FailureReason { get; private init; }

    public static StripeWebhookSignatureValidationResult Valid() =>
        new() { IsValid = true };

    public static StripeWebhookSignatureValidationResult Invalid(string reason) =>
        new() { IsValid = false, FailureReason = reason };
}
