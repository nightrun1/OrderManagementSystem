namespace OrderManagementSystem.DTOs.Payments;

public record PaymentDetailsDto(
    int Id,
    int OrderId,
    decimal Amount,
    string Provider,
    string Status,
    string TransactionId,
    DateTime CreatedAt);
