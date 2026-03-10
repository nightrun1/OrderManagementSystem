using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

public class OrderRepository(AppDbContext context) : BaseRepository<Order>(context), IOrderRepository
{
    public async Task<IEnumerable<Order>> GetByUserIdAsync(int userId)
    {
        return await Context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Order>> GetByStatusAsync(OrderStatus status)
    {
        return await Context.Orders
            .Where(o => o.Status == status)
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync();
    }

    public async Task<Order?> GetWithItemsAsync(int orderId)
    {
        return await Context.Orders
            .Include(o => o.Items)
            .ThenInclude(oi => oi.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);
    }
}
