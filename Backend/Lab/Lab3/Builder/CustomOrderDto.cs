using OrderManagementSystem.DTOs.Orders;

namespace OrderManagementSystem.Lab.Lab3.Builder;

public class CustomOrderDto
{
    public int UserId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];
    public string ShippingAddress { get; set; } = string.Empty;
    public string? DiscountCode { get; set; }
    public bool IsPriority { get; set; }
    public string? CustomerNote { get; set; }
    public string DeliveryOption { get; set; } = "standard";
    public decimal FinalTotal { get; set; }
    public decimal ShippingCost { get; set; }
    public decimal DiscountAmount { get; set; }
}
