using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.FactoryMethod;

public interface IOrder
{
    int UserId { get; set; }
    string ShippingAddress { get; set; }
    OrderStatus Status { get; set; }
    decimal TotalAmount { get; set; }
    string OrderType { get; }
    void Validate();
    decimal CalculateShippingCost();
}
