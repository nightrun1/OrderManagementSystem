using OrderManagementSystem.Models;

namespace OrderManagementSystem.Interfaces;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByOrderIdAsync(int orderId);
    Task<IEnumerable<Payment>> GetByProviderAsync(string provider);
}
