using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Controllers;

// TODO: Implement Discounts API controller (REST endpoints)
[ApiController]
[Route("api/[controller]")]
public class DiscountsController : ControllerBase
{
    // TODO: IDiscountService field (injected)
    // TODO: Constructor with service injection (DIP)
    
    // TODO: GET api/discounts/validate/{code} - Validate discount code endpoint
    [HttpGet("validate/{code}")]
    public Task<IActionResult> ValidateDiscount(string code)
    {
        throw new NotImplementedException();
    }
    
    // TODO: POST api/discounts/apply - Apply discount to order endpoint
    [HttpPost("apply")]
    public Task<IActionResult> ApplyDiscount()
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/discounts/{code}/price - Calculate discounted price endpoint
    [HttpGet("{code}/price")]
    public Task<IActionResult> GetDiscountedPrice(string code)
    {
        throw new NotImplementedException();
    }
}
