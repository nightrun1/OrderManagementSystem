using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Lab.Lab4.Adapter;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/shipping")]
public class ShippingController(ShippingService shippingService) : ControllerBase
{
    [HttpGet("quotes")]
    public async Task<ActionResult<List<ShippingQuote>>> GetQuotes(
        [FromQuery] string fromCity,
        [FromQuery] string toCity,
        [FromQuery] double weightKg)
    {
        if (string.IsNullOrWhiteSpace(fromCity) || string.IsNullOrWhiteSpace(toCity) || weightKg <= 0)
            return BadRequest(new { message = "fromCity, toCity si weightKg (>0) sunt obligatorii." });

        var quotes = await shippingService.GetAllQuotesAsync(fromCity, toCity, weightKg);
        return Ok(quotes);
    }

    [HttpPost("create")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<object>> CreateShipment([FromBody] CreateShipmentRequest request)
    {
        var order = new Models.Order
        {
            Id = request.OrderId,
            UserId = GetCurrentUserId(),
            ShippingAddress = request.ShippingAddress ?? "Default Address"
        };

        try
        {
            var trackingNumber = await shippingService.CreateShipmentAsync(request.ProviderName, order);
            return Ok(new { trackingNumber, provider = request.ProviderName });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("track")]
    public async Task<ActionResult<ShippingTrackingInfo>> Track(
        [FromQuery] string provider,
        [FromQuery] string trackingNumber)
    {
        if (string.IsNullOrWhiteSpace(provider) || string.IsNullOrWhiteSpace(trackingNumber))
            return BadRequest(new { message = "provider si trackingNumber sunt obligatorii." });

        try
        {
            var info = await shippingService.TrackAsync(provider, trackingNumber);
            return Ok(info);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : 0;
    }
}

public record CreateShipmentRequest(int OrderId, string ProviderName, string? ShippingAddress);
