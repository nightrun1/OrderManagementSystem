using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Interfaces.Services;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Services;

// TODO: Implement customer service with single responsibility (SRP)
public class CustomerService : ICustomerService
{
    // TODO: ICustomerRepository field (injected dependency)
    // TODO: Constructor with DIP
    
    // TODO: CreateCustomerAsync implementation
    public Task<Customer> CreateCustomerAsync(Customer customer)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetCustomerByIdAsync implementation
    public Task<Customer?> GetCustomerByIdAsync(int customerId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: UpdateCustomerAsync implementation
    public Task<bool> UpdateCustomerAsync(Customer customer)
    {
        throw new NotImplementedException();
    }
    
    // TODO: DeleteCustomerAsync implementation
    public Task<bool> DeleteCustomerAsync(int customerId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetCustomerOrdersAsync implementation
    public Task<IEnumerable<Order>> GetCustomerOrdersAsync(int customerId)
    {
        throw new NotImplementedException();
    }
}
