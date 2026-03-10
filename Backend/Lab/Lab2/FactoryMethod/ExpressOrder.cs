using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public class ExpressOrder(CreateOrderRequest request) : IOrder
{
    public int UserId { get; set; }
    public string ShippingAddress { get; set; } = request.ShippingAddress?.Trim() ?? string.Empty;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string OrderType => "Express";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ShippingAddress))
        {
            throw new InvalidOperationException("Shipping address is required for express orders.");
        }

        if (TotalAmount <= 0)
        {
            throw new InvalidOperationException("Express orders must have a positive total amount.");
        }
    }

    public decimal CalculateShippingCost() => 45.00m;
}
