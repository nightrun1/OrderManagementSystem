using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public class StandardOrder(CreateOrderRequest request) : IOrder
{
    public int UserId { get; set; }
    public string ShippingAddress { get; set; } = request.ShippingAddress?.Trim() ?? string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string OrderType => "Standard";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ShippingAddress))
        {
            throw new InvalidOperationException("Shipping address is required for standard orders.");
        }
    }

    public decimal CalculateShippingCost() => 15.00m;
}
