using OrderManagementSystem.DTOs.Orders;

namespace OrderManagementSystem.Lab.Lab3.Builder;

public class OrderDirector(ICustomOrderBuilder builder)
{
    public CustomOrderDto BuildQuickOrder(int userId, int productId, int quantity, string address)
    {
        return builder
            .ForUser(userId)
            .AddItem(productId, quantity)
            .ShipTo(address)
            .WithDeliveryOption("standard")
            .Build();
    }

    public CustomOrderDto BuildPriorityOrder(int userId, List<OrderItemRequest> items, string address)
    {
        builder.ForUser(userId);

        foreach (var item in items)
        {
            builder.AddItem(item.ProductId, item.Quantity);
        }

        return builder
            .ShipTo(address)
            .AsPriority()
            .WithDeliveryOption("express")
            .Build();
    }
}
