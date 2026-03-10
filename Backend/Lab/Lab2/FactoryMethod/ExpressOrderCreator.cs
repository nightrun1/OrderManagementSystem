using OrderManagementSystem.DTOs.Orders;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public class ExpressOrderCreator : OrderCreator
{
    public override IOrder CreateOrder(CreateOrderRequest request) => new ExpressOrder(request);
}
