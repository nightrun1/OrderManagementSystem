using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

// TODO: Implement product-specific repository extending generic repository (OCP)
public class ProductRepository : Repository<Product>, IProductRepository
{
    // TODO: Constructor calling base constructor with DbContext
    
    // TODO: GetProductsByCategoryAsync implementation
    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetProductsByPriceRangeAsync implementation
    public Task<IEnumerable<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
    {
        throw new NotImplementedException();
    }
    
    // TODO: SearchProductsAsync implementation (name/description search)
    public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }
}
