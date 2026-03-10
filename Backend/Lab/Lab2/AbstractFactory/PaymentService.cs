using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public class PaymentService(
    IPaymentProviderFactory factory,
    IOrderRepository orderRepository,
    IPaymentRepository paymentRepository)
{
    public async Task<PaymentProcessResult> ProcessPaymentAsync(int orderId, decimal amount, string cardToken)
    {
        var order = await orderRepository.GetWithItemsAsync(orderId);
        if (order is null)
        {
            throw new KeyNotFoundException($"Order {orderId} was not found.");
        }

        var chargeAmount = amount > 0 ? amount : order.TotalAmount;
        var processor = factory.CreateProcessor();

        PaymentResult paymentResult;
        try
        {
            paymentResult = await processor.ChargeAsync(chargeAmount, "RON", cardToken);
        }
        catch (Exception ex)
        {
            paymentResult = new PaymentResult(false, string.Empty, chargeAmount, "RON", ex.Message);
        }

        var receiptText = string.Empty;
        if (paymentResult.Success)
        {
            var receiptGenerator = factory.CreateReceiptGenerator();
            receiptText = receiptGenerator.Generate(paymentResult, order);
        }

        await SavePaymentAsync(orderId, chargeAmount, paymentResult);

        return new PaymentProcessResult(paymentResult, receiptText, factory.ProviderName);
    }

    public string GetProviderName() => factory.ProviderName;

    private async Task SavePaymentAsync(int orderId, decimal amount, PaymentResult result)
    {
        var payment = await paymentRepository.GetByOrderIdAsync(orderId);
        var status = result.Success ? "Completed" : "Failed";
        var transactionId = string.IsNullOrWhiteSpace(result.TransactionId)
            ? $"failed-{Guid.NewGuid():N}"
            : result.TransactionId;

        if (payment is null)
        {
            payment = new Payment
            {
                OrderId = orderId,
                Amount = amount,
                Provider = factory.ProviderName,
                Status = status,
                TransactionId = transactionId
            };

            await paymentRepository.AddAsync(payment);
            return;
        }

        payment.Amount = amount;
        payment.Provider = factory.ProviderName;
        payment.Status = status;
        payment.TransactionId = transactionId;

        await paymentRepository.UpdateAsync(payment);
    }
}
