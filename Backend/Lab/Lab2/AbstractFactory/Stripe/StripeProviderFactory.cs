namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.Stripe;

public class StripeProviderFactory : IPaymentProviderFactory
{
    public string ProviderName => "Stripe";

    public IPaymentProcessor CreateProcessor() => new StripePaymentProcessor();

    public IReceiptGenerator CreateReceiptGenerator() => new StripeReceiptGenerator();

    public IWebhookParser CreateWebhookParser() => new StripeWebhookParser();
}
