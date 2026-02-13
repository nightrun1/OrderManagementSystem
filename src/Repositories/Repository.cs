using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

// TODO: Implement generic repository with CRUD operations (DIP implementation)
public class Repository<T> : IRepository<T> where T : class
{
    // TODO: DbContext field
    // TODO: DbSet property for entity type
    // TODO: Constructor with DbContext injection
    
    // TODO: GetByIdAsync implementation
    public Task<T?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetAllAsync implementation
    public Task<IEnumerable<T>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
    
    // TODO: AddAsync implementation
    public Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }
    
    // TODO: UpdateAsync implementation
    public Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }
    
    // TODO: DeleteAsync implementation
    public Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }
    
    // TODO: SaveChangesAsync implementation
    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
