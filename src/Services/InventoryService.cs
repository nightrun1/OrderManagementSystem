using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Interfaces.Services;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

// TODO: Implement inventory service with single responsibility (SRP)
public class InventoryService : IInventoryService
{
    // TODO: IInventoryRepository field (injected dependency)
    // TODO: Constructor with DIP
    
    // TODO: ReserveStockAsync implementation
    public Task<bool> ReserveStockAsync(int productId, int quantity)
    {
        throw new NotImplementedException();
    }
    
    // TODO: ReleaseReservationAsync implementation
    public Task<bool> ReleaseReservationAsync(int productId, int quantity)
    {
        throw new NotImplementedException();
    }
    
    // TODO: UpdateStockAsync implementation
    public Task<bool> UpdateStockAsync(int productId, int quantityChange)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetAvailableStockAsync implementation
    public Task<int> GetAvailableStockAsync(int productId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: CheckProductAvailabilityAsync implementation
    public Task<bool> CheckProductAvailabilityAsync(int productId, int requiredQuantity)
    {
        throw new NotImplementedException();
    }
}
