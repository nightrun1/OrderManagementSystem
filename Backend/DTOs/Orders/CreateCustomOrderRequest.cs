namespace OrderManagementSystem.DTOs.Orders;

public class CreateCustomOrderRequest
{
    public int UserId { get; set; }
    public List<OrderItemRequest> Items { get; set; } = [];
    public string ShippingAddress { get; set; } = string.Empty;
    public string? DiscountCode { get; set; }
    public bool IsPriority { get; set; }
    public string? Note { get; set; }
    public string DeliveryOption { get; set; } = "standard";
    public bool UseDirectorPreset { get; set; }
    public string? PresetType { get; set; }
}
