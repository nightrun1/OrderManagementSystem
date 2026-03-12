using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Lab.Lab4.Facade;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/checkout")]
[Authorize]
public class CheckoutController(OrderPlacementFacade facade) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<PlaceOrderResult>> Checkout([FromBody] PlaceOrderRequest request)
    {
        request.UserId = GetCurrentUserId();

        var result = await facade.PlaceOrderAsync(request);

        if (result.Success)
            return StatusCode(201, result);

        return BadRequest(result);
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : 0;
    }
}
