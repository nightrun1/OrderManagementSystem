using OrderManagementSystem.Models;

namespace OrderManagementSystem.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<IEnumerable<Order>> GetByUserIdAsync(int userId);
    Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status);
    Task<Order?> GetWithItemsAsync(int orderId);
}
