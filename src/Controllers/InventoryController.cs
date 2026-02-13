using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Controllers;

// TODO: Implement Inventory API controller (REST endpoints)
[ApiController]
[Route("api/[controller]")]
public class InventoryController : ControllerBase
{
    // TODO: IInventoryService field (injected)
    // TODO: Constructor with service injection (DIP)
    
    // TODO: GET api/inventory/product/{productId} - Get available stock endpoint
    [HttpGet("product/{productId}")]
    public Task<IActionResult> GetAvailableStock(int productId)
    {
        throw new NotImplementedException();
    }
    
    // TODO: POST api/inventory/reserve - Reserve stock endpoint
    [HttpPost("reserve")]
    public Task<IActionResult> ReserveStock()
    {
        throw new NotImplementedException();
    }
    
    // TODO: POST api/inventory/release - Release reservation endpoint
    [HttpPost("release")]
    public Task<IActionResult> ReleaseReservation()
    {
        throw new NotImplementedException();
    }
    
    // TODO: PUT api/inventory/update - Update stock endpoint
    [HttpPut("update")]
    public Task<IActionResult> UpdateStock()
    {
        throw new NotImplementedException();
    }
}
