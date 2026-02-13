using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Services;

// TODO: Implement notification service with single responsibility (SRP)
public class NotificationService : INotificationService
{
    // TODO: Email service interface (to be defined)
    // TODO: SMS service interface (to be defined)
    // TODO: Constructor with DIP
    
    // TODO: SendOrderConfirmationAsync implementation
    public Task<bool> SendOrderConfirmationAsync(int orderId, string customerEmail)
    {
        throw new NotImplementedException();
    }
    
    // TODO: SendOrderShippedAsync implementation
    public Task<bool> SendOrderShippedAsync(int orderId, string customerEmail)
    {
        throw new NotImplementedException();
    }
    
    // TODO: SendPaymentReceiptAsync implementation
    public Task<bool> SendPaymentReceiptAsync(int paymentId, string customerEmail)
    {
        throw new NotImplementedException();
    }
    
    // TODO: SendOrderCancelledAsync implementation
    public Task<bool> SendOrderCancelledAsync(int orderId, string customerEmail)
    {
        throw new NotImplementedException();
    }
}
