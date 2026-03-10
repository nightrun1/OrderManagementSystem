namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public interface IPaymentProcessor
{
    Task<PaymentResult> ChargeAsync(decimal amount, string currency, string cardToken);
    Task<bool> RefundAsync(string transactionId, decimal amount);
    string ProviderName { get; }
}
