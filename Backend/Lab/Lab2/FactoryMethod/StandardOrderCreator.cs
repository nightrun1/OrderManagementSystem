using OrderManagementSystem.DTOs.Orders;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public class StandardOrderCreator : OrderCreator
{
    public override IOrder CreateOrder(CreateOrderRequest request) => new StandardOrder(request);
}
