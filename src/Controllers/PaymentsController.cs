using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Controllers;

// TODO: Implement Payments API controller (REST endpoints)
[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    // TODO: IPaymentService field (injected)
    // TODO: Constructor with service injection (DIP)
    
    // TODO: POST api/payments - Process payment endpoint
    [HttpPost]
    public Task<IActionResult> ProcessPayment()
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/payments/{id} - Get payment status endpoint
    [HttpGet("{id}")]
    public Task<IActionResult> GetPaymentStatus(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: POST api/payments/{id}/refund - Refund payment endpoint
    [HttpPost("{id}/refund")]
    public Task<IActionResult> RefundPayment(int id)
    {
        throw new NotImplementedException();
    }
}
