using OrderManagementSystem.Interfaces.Services;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

// TODO: Implement discount service with single responsibility (SRP)
public class DiscountService : IDiscountService
{
    // TODO: Discount repository field (to be defined)
    // TODO: Constructor with DIP
    
    // TODO: ApplyDiscountAsync implementation
    public Task<bool> ApplyDiscountAsync(int orderId, string discountCode)
    {
        throw new NotImplementedException();
    }
    
    // TODO: ValidateDiscountAsync implementation
    public Task<bool> ValidateDiscountAsync(string discountCode)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetDiscountByCodeAsync implementation
    public Task<Discount?> GetDiscountByCodeAsync(string code)
    {
        throw new NotImplementedException();
    }
    
    // TODO: CalculateDiscountedPriceAsync implementation
    public Task<decimal> CalculateDiscountedPriceAsync(decimal originalPrice, string discountCode)
    {
        throw new NotImplementedException();
    }
}
