using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Interfaces.Services;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

// TODO: Implement order service with single responsibility (SRP)
public class OrderService : IOrderService
{
    // TODO: IOrderRepository field (injected dependency)
    // TODO: IInventoryService field (injected dependency)
    // TODO: INotificationService field (injected dependency)
    // TODO: Constructor with DIP - inject interfaces not implementations
    
    // TODO: CreateOrderAsync implementation
    public Task<Order> CreateOrderAsync(Order order)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetOrderByIdAsync implementation
    public Task<Order?> GetOrderByIdAsync(int orderId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetOrdersByCustomerAsync implementation
    public Task<IEnumerable<Order>> GetOrdersByCustomerAsync(int customerId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: CancelOrderAsync implementation
    public Task<bool> CancelOrderAsync(int orderId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: UpdateOrderStatusAsync implementation
    public Task<bool> UpdateOrderStatusAsync(int orderId, OrderStatus status)
    {
        throw new NotImplementedException();
    }
}
