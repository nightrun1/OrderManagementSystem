using OrderManagementSystem.Models;

namespace OrderManagementSystem.DTOs.Orders;

public record CustomOrderResultDto(
    int OrderId,
    OrderStatus Status,
    decimal FinalTotal,
    decimal ShippingCost,
    decimal DiscountAmount,
    string DeliveryOption,
    bool IsPriority,
    string ShippingAddress,
    DateTime CreatedAt,
    List<OrderItemDto> Items,
    string? CustomerNote);
