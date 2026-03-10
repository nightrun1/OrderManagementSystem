using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public interface IReceiptGenerator
{
    string Generate(PaymentResult payment, Order order);
    string Format { get; }
}
