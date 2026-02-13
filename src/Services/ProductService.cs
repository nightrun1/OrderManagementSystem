using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Interfaces.Services;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

// TODO: Implement product service with single responsibility (SRP)
public class ProductService : IProductService
{
    // TODO: IProductRepository field (injected dependency)
    // TODO: Constructor with DIP
    
    // TODO: GetProductByIdAsync implementation
    public Task<Product?> GetProductByIdAsync(int productId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetAllProductsAsync implementation
    public Task<IEnumerable<Product>> GetAllProductsAsync()
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetProductsByCategoryAsync implementation
    public Task<IEnumerable<Product>> GetProductsByCategoryAsync(ProductCategory category)
    {
        throw new NotImplementedException();
    }
    
    // TODO: SearchProductsAsync implementation
    public Task<IEnumerable<Product>> SearchProductsAsync(string searchTerm)
    {
        throw new NotImplementedException();
    }
    
    // TODO: CreateProductAsync implementation
    public Task<Product> CreateProductAsync(Product product)
    {
        throw new NotImplementedException();
    }
    
    // TODO: UpdateProductAsync implementation
    public Task<bool> UpdateProductAsync(Product product)
    {
        throw new NotImplementedException();
    }
}
