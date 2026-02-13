using OrderManagementSystem.Interfaces.Repositories;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Repositories;

// TODO: Implement customer-specific repository extending generic repository (OCP)
public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    // TODO: Constructor calling base constructor with DbContext
    
    // TODO: GetCustomerByEmailAsync implementation
    public Task<Customer?> GetCustomerByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GetCustomerWithOrdersAsync implementation with eager loading
    public Task<Customer?> GetCustomerWithOrdersAsync(int customerId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: CustomerExistsByEmailAsync implementation
    public Task<bool> CustomerExistsByEmailAsync(string email)
    {
        throw new NotImplementedException();
    }
}
