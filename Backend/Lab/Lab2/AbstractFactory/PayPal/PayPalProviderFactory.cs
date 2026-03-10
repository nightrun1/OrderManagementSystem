namespace OrderManagementSystem.Lab.Lab2.AbstractFactory.PayPal;

public class PayPalProviderFactory : IPaymentProviderFactory
{
    public string ProviderName => "PayPal";

    public IPaymentProcessor CreateProcessor() => new PayPalPaymentProcessor();

    public IReceiptGenerator CreateReceiptGenerator() => new PayPalReceiptGenerator();

    public IWebhookParser CreateWebhookParser() => new PayPalWebhookParser();
}
