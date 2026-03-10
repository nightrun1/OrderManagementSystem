using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

public class PaymentRepository(AppDbContext context) : BaseRepository<Payment>(context), IPaymentRepository
{
    public async Task<Payment?> GetByOrderIdAsync(int orderId)
    {
        return await Context.Payments.FirstOrDefaultAsync(p => p.OrderId == orderId);
    }

    public async Task<IEnumerable<Payment>> GetByProviderAsync(string provider)
    {
        return await Context.Payments
            .Where(p => p.Provider == provider)
            .ToListAsync();
    }
}
