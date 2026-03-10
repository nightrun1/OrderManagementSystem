using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.Stripe;

public class StripeWebhookParser : IWebhookParser
{
    public WebhookEvent Parse(string rawPayload, string signature)
    {
        using var document = JsonDocument.Parse(rawPayload);
        var root = document.RootElement;

        var eventType = GetString(root, "type") ?? GetString(root, "eventType") ?? "unknown";
        var transactionId = GetTransactionId(root);
        var amount = GetAmount(root);
        var metadata = GetMetadata(root);

        return new WebhookEvent(eventType, transactionId, amount, metadata);
    }

    public bool ValidateSignature(string payload, string signature, string secret)
    {
        var provided = signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase)
            ? signature[7..]
            : signature;

        var computed = ComputeHmacSha256(payload, secret);
        return string.Equals(computed, provided, StringComparison.OrdinalIgnoreCase);
    }

    private static string ComputeHmacSha256(string payload, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        var data = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(data);

        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    private static string GetTransactionId(JsonElement root)
    {
        if (root.TryGetProperty("data", out var data) &&
            data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("object", out var obj) &&
            obj.ValueKind == JsonValueKind.Object)
        {
            return GetString(obj, "id") ?? string.Empty;
        }

        return GetString(root, "transactionId") ?? string.Empty;
    }

    private static decimal GetAmount(JsonElement root)
    {
        if (root.TryGetProperty("data", out var data) &&
            data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("object", out var obj) &&
            obj.ValueKind == JsonValueKind.Object &&
            obj.TryGetProperty("amount", out var amountElement) &&
            amountElement.ValueKind == JsonValueKind.Number &&
            amountElement.TryGetDecimal(out var amount))
        {
            return amount;
        }

        return 0m;
    }

    private static Dictionary<string, string> GetMetadata(JsonElement root)
    {
        var metadata = new Dictionary<string, string>();

        if (root.TryGetProperty("data", out var data) &&
            data.ValueKind == JsonValueKind.Object &&
            data.TryGetProperty("object", out var obj) &&
            obj.ValueKind == JsonValueKind.Object &&
            obj.TryGetProperty("metadata", out var metadataElement) &&
            metadataElement.ValueKind == JsonValueKind.Object)
        {
            foreach (var property in metadataElement.EnumerateObject())
            {
                metadata[property.Name] = property.Value.ToString();
            }
        }

        return metadata;
    }

    private static string? GetString(JsonElement element, string propertyName)
    {
        if (element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String)
        {
            return value.GetString();
        }

        return null;
    }
}
