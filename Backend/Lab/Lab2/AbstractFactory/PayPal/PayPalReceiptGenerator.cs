using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.PayPal;

public class PayPalReceiptGenerator : IReceiptGenerator
{
    public string Format => "paypal-receipt";

    public string Generate(PaymentResult payment, Order order)
    {
        return $"--- PayPal Payment Confirmation ---\\nTxn: {payment.TransactionId}\\nPaid: {payment.Amount:0.00} {payment.Currency}\\nOrder Ref: {order.Id}";
    }
}
