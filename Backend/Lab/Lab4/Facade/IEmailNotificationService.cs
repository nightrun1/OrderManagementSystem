using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab4.Facade;

public interface IEmailNotificationService
{
    Task SendOrderConfirmationAsync(Order order);
    Task SendStatusUpdateAsync(Order order, OrderStatus newStatus);
}
