using System.Security.Cryptography;
using System.Text;
using FluentAssertions;
using Mistruna.Core.Payments.Stripe;
using Xunit;

namespace Mistruna.Core.Payments.Stripe.Tests;

public class StripeWebhookSignatureValidatorTests
{
    private const string Secret = "whsec_test_secret";

    [Fact]
    public void Validate_WithCorrectSignature_ReturnsValid()
    {
        var payload = "{\"id\":\"evt_1\"}";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signedPayload = $"{timestamp}.{payload}";
        var signature = ComputeHmac(signedPayload, Secret);
        var header = $"t={timestamp},v1={signature}";

        var validator = new StripeWebhookSignatureValidator();

        var result = validator.Validate(payload, header, Secret, TimeSpan.FromSeconds(300));

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WithTamperedPayload_ReturnsInvalid()
    {
        var payload = "{\"id\":\"evt_1\"}";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signedPayload = $"{timestamp}.{payload}";
        var signature = ComputeHmac(signedPayload, Secret);
        var header = $"t={timestamp},v1={signature}";

        var validator = new StripeWebhookSignatureValidator();

        var result = validator.Validate("{\"id\":\"evt_2\"}", header, Secret, TimeSpan.FromSeconds(300));

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("mismatch");
    }

    [Fact]
    public void Validate_WithStaleTimestamp_ReturnsInvalid()
    {
        var payload = "{\"id\":\"evt_1\"}";
        var staleTs = DateTimeOffset.UtcNow.AddMinutes(-10).ToUnixTimeSeconds();
        var signedPayload = $"{staleTs}.{payload}";
        var signature = ComputeHmac(signedPayload, Secret);
        var header = $"t={staleTs},v1={signature}";

        var validator = new StripeWebhookSignatureValidator();

        var result = validator.Validate(payload, header, Secret, TimeSpan.FromSeconds(300));

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("tolerance");
    }

    [Fact]
    public void Validate_WithMalformedHeader_ReturnsInvalid()
    {
        var validator = new StripeWebhookSignatureValidator();

        var result = validator.Validate("payload", "garbage", Secret, TimeSpan.FromSeconds(300));

        result.IsValid.Should().BeFalse();
        result.FailureReason.Should().Contain("malformed");
    }

    [Fact]
    public void Validate_WithMultipleV1Signatures_AcceptsAnyValidOne()
    {
        var payload = "{\"id\":\"evt_1\"}";
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var signedPayload = $"{timestamp}.{payload}";
        var goodSignature = ComputeHmac(signedPayload, Secret);
        var header = $"t={timestamp},v1=deadbeef,v1={goodSignature}";

        var validator = new StripeWebhookSignatureValidator();

        var result = validator.Validate(payload, header, Secret, TimeSpan.FromSeconds(300));

        result.IsValid.Should().BeTrue();
    }

    private static string ComputeHmac(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
