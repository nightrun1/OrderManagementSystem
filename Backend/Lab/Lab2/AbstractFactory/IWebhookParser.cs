namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public interface IWebhookParser
{
    WebhookEvent Parse(string rawPayload, string signature);
    bool ValidateSignature(string payload, string signature, string secret);
}
