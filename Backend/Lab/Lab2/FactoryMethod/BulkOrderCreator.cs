using OrderManagementSystem.DTOs.Orders;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public class BulkOrderCreator : OrderCreator
{
    public override IOrder CreateOrder(CreateOrderRequest request) => new BulkOrder(request, minimumQuantity: 10);
}
