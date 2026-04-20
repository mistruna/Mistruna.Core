using FluentAssertions;
using Mistruna.Core.Payments.Stripe;
using Mistruna.Core.Payments.Stripe.Events;
using Xunit;

namespace Mistruna.Core.Payments.Stripe.Tests;

public class StripeEventParserTests
{
    private static readonly StripeEventParser Parser = new();

    [Fact]
    public void Parse_SubscriptionCreated_ProducesTypedRecord()
    {
        var json = """
        {
          "id": "evt_1",
          "type": "customer.subscription.created",
          "created": 1700000000,
          "data": {
            "object": {
              "id": "sub_123",
              "customer": "cus_456",
              "status": "active",
              "current_period_start": 1700000000,
              "current_period_end": 1702592000,
              "items": { "data": [ { "price": { "id": "price_pro_monthly" } } ] }
            }
          }
        }
        """;

        var result = Parser.Parse(json);

        result.Should().BeOfType<StripeSubscriptionCreated>()
              .Which.StripeSubscriptionId.Should().Be("sub_123");
        ((StripeSubscriptionCreated)result).StripeCustomerId.Should().Be("cus_456");
        ((StripeSubscriptionCreated)result).StripePriceId.Should().Be("price_pro_monthly");
    }

    [Fact]
    public void Parse_InvoicePaymentFailed_ProducesTypedRecord()
    {
        var json = """
        {
          "id": "evt_inv",
          "type": "invoice.payment_failed",
          "created": 1700000000,
          "data": {
            "object": {
              "id": "in_1",
              "customer": "cus_456",
              "amount_due": 4900,
              "currency": "usd"
            }
          }
        }
        """;

        var result = Parser.Parse(json);

        result.Should().BeOfType<StripeInvoicePaymentFailed>()
              .Which.AmountDue.Should().Be(4900);
    }

    [Fact]
    public void Parse_UnknownEventType_ProducesUnknownEvent_WithRawJson()
    {
        var json = """
        {
          "id": "evt_xyz",
          "type": "charge.refund.updated",
          "created": 1700000000,
          "data": { "object": {} }
        }
        """;

        var result = Parser.Parse(json);

        result.Should().BeOfType<StripeUnknownEvent>()
              .Which.RawJson.Should().Contain("charge.refund.updated");
    }

    [Fact]
    public void Parse_InvalidJson_Throws()
    {
        var action = () => Parser.Parse("{ not json");
        action.Should().Throw<Exception>();
    }
}
