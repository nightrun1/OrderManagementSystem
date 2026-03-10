namespace OrderManagementSystem.Lab.Lab2.AbstractFactory;

public interface IPaymentProviderFactory
{
    IPaymentProcessor CreateProcessor();
    IReceiptGenerator CreateReceiptGenerator();
    IWebhookParser CreateWebhookParser();
    string ProviderName { get; }
}
