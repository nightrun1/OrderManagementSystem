namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.Stripe;

public class StripePaymentProcessor : IPaymentProcessor
{
    public string ProviderName => "Stripe";

    public Task<PaymentResult> ChargeAsync(decimal amount, string currency, string cardToken)
    {
        if (cardToken.Contains("fail", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Stripe rejected the payment token.");
        }

        var result = new PaymentResult(
            true,
            $"str_{Guid.NewGuid():N}",
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
