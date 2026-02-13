using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

// TODO: Implement inventory-specific repository extending generic repository (OCP)
public class InventoryRepository : Repository<Inventory>, IInventoryRepository
{
    // TODO: Constructor calling base constructor with DbContext
    
    // TODO: GetInventoryByProductIdAsync implementation
    public Task<Inventory?> GetInventoryByProductIdAsync(int productId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: UpdateStockAsync implementation (increase or decrease quantity)
    public Task UpdateStockAsync(int productId, int quantityChange)
    {
        throw new NotImplementedException();
    }
    
    // TODO: CheckStockAvailabilityAsync implementation
    public Task<bool> CheckStockAvailabilityAsync(int productId, int requiredQuantity)
    {
        throw new NotImplementedException();
    }
}
