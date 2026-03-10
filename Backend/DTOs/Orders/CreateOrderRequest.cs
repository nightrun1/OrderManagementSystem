namespace OrderManagementSystem.DTOs.Orders;

public record CreateOrderRequest(string ShippingAddress, List<OrderItemRequest> Items, string OrderType = "standard");
