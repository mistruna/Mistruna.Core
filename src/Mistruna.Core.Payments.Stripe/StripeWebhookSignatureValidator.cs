using System.Security.Cryptography;
using System.Text;

namespace Mistruna.Core.Payments.Stripe;

/// <summary>
/// Validates the <c>Stripe-Signature</c> header. See
/// https://stripe.com/docs/webhooks/signatures for the protocol.
/// </summary>
/// <remarks>
/// Header format: <c>t={timestamp},v1={hex-sig}[,v1={hex-sig}...]</c>.
/// Signature: HMAC-SHA256(key=webhookSecret, data=$"{timestamp}.{rawPayload}").
/// The validator rejects timestamps outside <c>±tolerance</c> of server time to defend
/// against replay. Multiple <c>v1</c> entries are accepted when Stripe is rotating secrets.
/// </remarks>
public sealed class StripeWebhookSignatureValidator
{
    public StripeWebhookSignatureValidationResult Validate(
        string payload,
        string signatureHeader,
        string webhookSecret,
        TimeSpan tolerance)
    {
        if (string.IsNullOrWhiteSpace(signatureHeader))
        {
            return StripeWebhookSignatureValidationResult.Invalid("signature header is missing or malformed");
        }

        long? timestamp = null;
        var v1Signatures = new List<string>();

        foreach (var part in signatureHeader.Split(','))
        {
            var kv = part.Split('=', 2);
            if (kv.Length != 2) continue;

            switch (kv[0])
            {
                case "t" when long.TryParse(kv[1], out var ts):
                    timestamp = ts;
                    break;
                case "v1":
                    v1Signatures.Add(kv[1]);
                    break;
            }
        }

        if (timestamp is null || v1Signatures.Count == 0)
        {
            return StripeWebhookSignatureValidationResult.Invalid("signature header is malformed");
        }

        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        if (Math.Abs(now - timestamp.Value) > (long)tolerance.TotalSeconds)
        {
            return StripeWebhookSignatureValidationResult.Invalid("timestamp outside of tolerance window");
        }

        var signedPayload = $"{timestamp.Value}.{payload}";
        var expected = ComputeHmac(signedPayload, webhookSecret);

        foreach (var candidate in v1Signatures)
        {
            if (FixedTimeEquals(expected, candidate))
            {
                return StripeWebhookSignatureValidationResult.Valid();
            }
        }

        return StripeWebhookSignatureValidationResult.Invalid("signature mismatch");
    }

    private static string ComputeHmac(string data, string key)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static bool FixedTimeEquals(string a, string b)
    {
        var aBytes = Encoding.UTF8.GetBytes(a);
        var bBytes = Encoding.UTF8.GetBytes(b);
        if (aBytes.Length != bBytes.Length) return false;
        return CryptographicOperations.FixedTimeEquals(aBytes, bBytes);
    }
}
