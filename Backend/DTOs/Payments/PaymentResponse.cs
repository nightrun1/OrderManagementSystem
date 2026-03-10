namespace OrderManagementSystem.DTOs.Payments;

public record PaymentResponse(
    bool Success,
    string TransactionId,
    decimal Amount,
    string Currency,
    string ErrorMessage,
    string ReceiptText,
    string ProviderName);
