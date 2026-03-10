using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

public class ProductRepository(AppDbContext context) : BaseRepository<Product>(context), IProductRepository
{
    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await Context.Products.FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public async Task<IEnumerable<Product>> GetByCategoryAsync(string category)
    {
        return await Context.Products
            .Where(p => p.Category == category && p.IsActive)
            .ToListAsync();
    }

    public async Task<bool> IsInStockAsync(int productId, int quantity)
    {
        return await Context.Products.AnyAsync(p => p.Id == productId && p.Stock >= quantity && p.IsActive);
    }
}
