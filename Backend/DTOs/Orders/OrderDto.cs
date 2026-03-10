using OrderManagementSystem.Models;

namespace OrderManagementSystem.DTOs.Orders;

public record OrderDto(
    int Id,
    OrderStatus Status,
    decimal TotalAmount,
    string ShippingAddress,
    DateTime CreatedAt,
    List<OrderItemDto> Items,
    string OrderType = "Standard",
    decimal ShippingCost = 15.00m);
