using OrderManagementSystem.Models;

namespace OrderManagementSystem.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product?> GetBySkuAsync(string sku);
    Task<IEnumerable<Product>> GetByCategoryAsync(string category);
    Task<bool> IsInStockAsync(int productId, int quantity);
}
