using OrderManagementSystem.DTOs.Orders;

namespace OrderManagementSystem.Lab.Lab4.Facade;

public class PlaceOrderRequest
{
    public int UserId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];
    public string ShippingAddress { get; set; } = string.Empty;
    public string PaymentToken { get; set; } = string.Empty;
    public string PaymentProvider { get; set; } = "Stripe";
    public string? DiscountCode { get; set; }
    public string DeliveryOption { get; set; } = "FanCourier";
}
