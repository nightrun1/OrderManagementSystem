using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab4.Facade;

public class ConsoleEmailNotificationService : IEmailNotificationService
{
    public Task SendOrderConfirmationAsync(Order order)
    {
        Console.WriteLine($"[EMAIL] Confirmare comanda #{order.Id} trimisa la User #{order.UserId}");
        return Task.CompletedTask;
    }

    public Task SendStatusUpdateAsync(Order order, OrderStatus newStatus)
    {
        Console.WriteLine($"[EMAIL] Status actualizat pentru comanda #{order.Id}: {newStatus}");
        return Task.CompletedTask;
    }
}
