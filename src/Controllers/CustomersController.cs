using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Controllers;

// TODO: Implement Customers API controller (REST endpoints)
[ApiController]
[Route("api/[controller]")]
public class CustomersController : ControllerBase
{
    // TODO: ICustomerService field (injected)
    // TODO: Constructor with service injection (DIP)
    
    // TODO: POST api/customers - Create customer endpoint
    [HttpPost]
    public Task<IActionResult> CreateCustomer()
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/customers/{id} - Get customer by id endpoint
    [HttpGet("{id}")]
    public Task<IActionResult> GetCustomer(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: PUT api/customers/{id} - Update customer endpoint
    [HttpPut("{id}")]
    public Task<IActionResult> UpdateCustomer(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: DELETE api/customers/{id} - Delete customer endpoint
    [HttpDelete("{id}")]
    public Task<IActionResult> DeleteCustomer(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/customers/{id}/orders - Get customer orders endpoint
    [HttpGet("{id}/orders")]
    public Task<IActionResult> GetCustomerOrders(int id)
    {
        throw new NotImplementedException();
    }
}
