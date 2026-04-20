using Mistruna.Core.Payments.Stripe.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stripe;
using Stripe.Checkout;

namespace Mistruna.Core.Payments.Stripe;

/// <summary>
/// Parses a raw Stripe webhook JSON payload into a strongly-typed <see cref="StripeEvent"/>.
/// Uses Newtonsoft.Json (the same library Stripe.net relies on internally) to deserialize the
/// envelope, then deserializes the <c>data.object</c> into the appropriate Stripe SDK type and
/// projects it into our record hierarchy.
/// </summary>
public sealed class StripeEventParser
{
    public StripeEvent Parse(string json)
    {
        // Let Newtonsoft throw naturally on invalid JSON — satisfies the "Parse_InvalidJson_Throws" contract.
        var envelope = JObject.Parse(json);

        var id = envelope["id"]?.Value<string>() ?? string.Empty;
        var type = envelope["type"]?.Value<string>() ?? string.Empty;
        var createdUnix = envelope["created"]?.Value<long>() ?? 0L;
        var created = DateTimeOffset.FromUnixTimeSeconds(createdUnix);
        var dataObjectJson = envelope["data"]?["object"]?.ToString() ?? "{}";

        return type switch
        {
            "customer.subscription.created" => ToSubscriptionCreated(id, created, dataObjectJson),
            "customer.subscription.updated" => ToSubscriptionUpdated(id, created, dataObjectJson),
            "customer.subscription.deleted" => ToSubscriptionDeleted(id, created, dataObjectJson),
            "invoice.payment_failed" => ToInvoicePaymentFailed(id, created, dataObjectJson),
            "checkout.session.completed" => ToCheckoutSessionCompleted(id, created, dataObjectJson),
            _ => new StripeUnknownEvent(id, type, created, json)
        };
    }

    private static StripeSubscriptionCreated ToSubscriptionCreated(string id, DateTimeOffset created, string dataObjectJson)
    {
        var sub = JsonConvert.DeserializeObject<Subscription>(dataObjectJson)!;
        return new StripeSubscriptionCreated(
            Id: id,
            Created: created,
            StripeCustomerId: sub.CustomerId,
            StripeSubscriptionId: sub.Id,
            StripePriceId: sub.Items?.Data?.FirstOrDefault()?.Price?.Id ?? string.Empty,
            Status: sub.Status,
            CurrentPeriodStart: new DateTimeOffset(sub.CurrentPeriodStart, TimeSpan.Zero),
            CurrentPeriodEnd: new DateTimeOffset(sub.CurrentPeriodEnd, TimeSpan.Zero));
    }

    private static StripeSubscriptionUpdated ToSubscriptionUpdated(string id, DateTimeOffset created, string dataObjectJson)
    {
        var sub = JsonConvert.DeserializeObject<Subscription>(dataObjectJson)!;
        return new StripeSubscriptionUpdated(
            Id: id,
            Created: created,
            StripeCustomerId: sub.CustomerId,
            StripeSubscriptionId: sub.Id,
            StripePriceId: sub.Items?.Data?.FirstOrDefault()?.Price?.Id ?? string.Empty,
            Status: sub.Status,
            CurrentPeriodStart: new DateTimeOffset(sub.CurrentPeriodStart, TimeSpan.Zero),
            CurrentPeriodEnd: new DateTimeOffset(sub.CurrentPeriodEnd, TimeSpan.Zero),
            CancelAt: sub.CancelAt.HasValue ? new DateTimeOffset(sub.CancelAt.Value, TimeSpan.Zero) : null);
    }

    private static StripeSubscriptionDeleted ToSubscriptionDeleted(string id, DateTimeOffset created, string dataObjectJson)
    {
        var sub = JsonConvert.DeserializeObject<Subscription>(dataObjectJson)!;
        return new StripeSubscriptionDeleted(
            Id: id,
            Created: created,
            StripeCustomerId: sub.CustomerId,
            StripeSubscriptionId: sub.Id);
    }

    private static StripeInvoicePaymentFailed ToInvoicePaymentFailed(string id, DateTimeOffset created, string dataObjectJson)
    {
        var inv = JsonConvert.DeserializeObject<Invoice>(dataObjectJson)!;
        return new StripeInvoicePaymentFailed(
            Id: id,
            Created: created,
            StripeCustomerId: inv.CustomerId,
            StripeInvoiceId: inv.Id,
            AmountDue: inv.AmountDue,
            Currency: inv.Currency);
    }

    private static StripeCheckoutSessionCompleted ToCheckoutSessionCompleted(string id, DateTimeOffset created, string dataObjectJson)
    {
        var session = JsonConvert.DeserializeObject<Session>(dataObjectJson)!;
        return new StripeCheckoutSessionCompleted(
            Id: id,
            Created: created,
            StripeCustomerId: session.CustomerId,
            StripeSubscriptionId: session.SubscriptionId ?? string.Empty,
            ClientReferenceId: session.ClientReferenceId);
    }
}
