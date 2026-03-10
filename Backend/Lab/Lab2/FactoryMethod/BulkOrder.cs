using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public class BulkOrder : IOrder
{
    private readonly CreateOrderRequest _request;
    private readonly int _minimumQuantity;

    public BulkOrder(CreateOrderRequest request, int minimumQuantity)
    {
        _request = request;
        _minimumQuantity = minimumQuantity;
        ShippingAddress = request.ShippingAddress?.Trim() ?? string.Empty;
    }

    public int UserId { get; set; }
    public string ShippingAddress { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalAmount { get; set; }
    public string OrderType => "Bulk";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ShippingAddress))
        {
            throw new InvalidOperationException("Shipping address is required for bulk orders.");
        }

        var totalUnits = _request.Items.Sum(i => i.Quantity);
        if (totalUnits < _minimumQuantity)
        {
            throw new InvalidOperationException($"Bulk orders require at least {_minimumQuantity} units.");
        }
    }

    public decimal CalculateShippingCost() => 0m;
}
