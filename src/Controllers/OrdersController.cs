using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Controllers;

// TODO: Implement Orders API controller (REST endpoints)
[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    // TODO: IOrderService field (injected)
    // TODO: Constructor with service injection (DIP)
    
    // TODO: POST api/orders - Create order endpoint
    [HttpPost]
    public Task<IActionResult> CreateOrder()
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/orders/{id} - Get order by id endpoint
    [HttpGet("{id}")]
    public Task<IActionResult> GetOrder(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/orders/customer/{customerId} - Get customer orders endpoint
    [HttpGet("customer/{customerId}")]
    public Task<IActionResult> GetCustomerOrders(int customerId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: PUT api/orders/{id}/status - Update order status endpoint
    [HttpPut("{id}/status")]
    public Task<IActionResult> UpdateOrderStatus(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: DELETE api/orders/{id} - Cancel order endpoint
    [HttpDelete("{id}")]
    public Task<IActionResult> CancelOrder(int id)
    {
        throw new NotImplementedException();
    }
}
