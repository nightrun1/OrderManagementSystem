using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public abstract class OrderCreator
{
    public abstract IOrder CreateOrder(CreateOrderRequest request);

    public OrderCreationResult ProcessOrder(CreateOrderRequest request, int userId, decimal totalAmount)
    {
        var order = CreateOrder(request);
        order.UserId = userId;
        order.ShippingAddress = request.ShippingAddress?.Trim() ?? string.Empty;
        order.Status = OrderStatus.Pending;
        order.TotalAmount = totalAmount;

        order.Validate();

        var shippingCost = order.CalculateShippingCost();
        return new OrderCreationResult(order, shippingCost);
    }
}
