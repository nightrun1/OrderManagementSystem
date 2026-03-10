namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.PayPal;

public class PayPalPaymentProcessor : IPaymentProcessor
{
    public string ProviderName => "PayPal";

    public Task<PaymentResult> ChargeAsync(decimal amount, string currency, string cardToken)
    {
        if (cardToken.Contains("fail", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("PayPal rejected the payment token.");
        }

        var result = new PaymentResult(
            true,
            $"pp_{Guid.NewGuid():N}",
            amount,
            currency,
            string.Empty);

        return Task.FromResult(result);
    }

    public Task<bool> RefundAsync(string transactionId, decimal amount)
    {
        return Task.FromResult(true);
    }
}
