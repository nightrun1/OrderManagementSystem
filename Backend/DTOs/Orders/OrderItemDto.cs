namespace OrderManagementSystem.DTOs.Orders;

public record OrderItemDto(int ProductId, string ProductName, int Quantity, decimal UnitPrice);
