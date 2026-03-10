namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public record WebhookEvent(
    string EventType,
    string TransactionId,
    decimal Amount,
    Dictionary<string, string> Metadata);
