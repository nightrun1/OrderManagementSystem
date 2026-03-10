namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public record PaymentResult(
    bool Success,
    string TransactionId,
    decimal Amount,
    string Currency,
    string ErrorMessage);
