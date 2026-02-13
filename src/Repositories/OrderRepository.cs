using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

// TODO: Implement order-specific repository extending generic repository (OCP)
public class OrderRepository : Repository<Order>, IOrderRepository
{
    // TODO: Constructor calling base constructor with DbContext
    
    // TODO: GetOrdersByCustomerIdAsync implementation
    public Task<IEnumerable<Order>> GetOrdersByCustomerIdAsync(int customerId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetOrderByIdWithItemsAsync implementation with eager loading
    public Task<Order?> GetOrderByIdWithItemsAsync(int orderId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetOrdersByStatusAsync implementation
    public Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
    {
        throw new NotImplementedException();
    }
}
