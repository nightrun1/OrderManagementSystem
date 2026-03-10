namespace OrderManagementSystem.DTOs.Orders;

public record OrderStatisticsDto(
    int TotalOrders,
    decimal TotalRevenue,
    decimal AverageOrderValue,
    Dictionary<string, int> OrdersPerStatus,
    DateTime? LastRefreshed);
