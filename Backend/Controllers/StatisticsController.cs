using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Lab.Lab3.Singleton;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/statistics")]
[Authorize(Roles = "Admin")]
public class StatisticsController(StatisticsService statisticsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<OrderStatisticsDto>> GetStatistics()
    {
        var statistics = await statisticsService.GetStatisticsAsync();
        return Ok(MapToDto(statistics));
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<OrderStatisticsDto>> Refresh()
    {
        var statistics = await statisticsService.ForceRefreshAsync();
        return Ok(MapToDto(statistics));
    }

    private static OrderStatisticsDto MapToDto(OrderStatisticsCache statistics)
    {
        return new OrderStatisticsDto(
            statistics.TotalOrders,
            statistics.TotalRevenue,
            statistics.AverageOrderValue,
            new Dictionary<string, int>(statistics.OrdersPerStatus),
            statistics.LastRefreshed);
    }
}
