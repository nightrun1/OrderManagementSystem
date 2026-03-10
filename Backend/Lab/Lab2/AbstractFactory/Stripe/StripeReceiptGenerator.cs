using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.Stripe;

public class StripeReceiptGenerator : IReceiptGenerator
{
    public string Format => "stripe-receipt";

    public string Generate(PaymentResult payment, Order order)
    {
        return $"=== STRIPE RECEIPT ===\\nTransaction: {payment.TransactionId}\\nAmount: {payment.Amount:0.00} {payment.Currency}\\nOrder: {order.Id}";
    }
}
