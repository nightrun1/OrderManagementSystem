namespace OrderManagementSystem.DTOs.Orders;

public record OrderTemplateDto(
    int Id,
    string Name,
    int CreatedByUserId,
    string ShippingAddress,
    DateTime CreatedAt,
    int ItemCount,
    List<OrderItemSnapshotDto> Items);
