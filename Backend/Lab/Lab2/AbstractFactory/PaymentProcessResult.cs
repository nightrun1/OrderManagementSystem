namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public record PaymentProcessResult(
    PaymentResult Payment,
    string ReceiptText,
    string ProviderName);
