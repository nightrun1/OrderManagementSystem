namespace OrderManagementSystem.DTOs.Payments;

public record CreatePaymentRequest(int OrderId, string CardToken);
