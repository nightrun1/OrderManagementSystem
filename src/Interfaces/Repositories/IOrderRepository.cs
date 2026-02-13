namespace OrderManagementSystem.Interfaces.Repositories;

// TODO: Define order-specific repository interface extending IRepository (ISP)
public interface IOrderRepository : IRepository<Models.Order>
{
    // TODO: GetOrdersByCustomerIdAsync method
    // TODO: GetOrderByIdWithItemsAsync method (include order items)
    // TODO: GetOrdersByStatusAsync method
}
