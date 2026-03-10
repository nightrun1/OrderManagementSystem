namespace OrderManagementSystem.DTOs.Orders;

public record OrderItemSnapshotDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice);
