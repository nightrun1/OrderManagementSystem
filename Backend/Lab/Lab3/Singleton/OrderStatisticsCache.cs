using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab3.Singleton;

public sealed class OrderStatisticsCache
{
    private static readonly Lazy<OrderStatisticsCache> _instance =
        new(() => new OrderStatisticsCache());

    public static OrderStatisticsCache Instance => _instance.Value;

    private readonly object _lock = new();

    private OrderStatisticsCache()
    {
    }

    public int TotalOrders { get; private set; }
    public decimal TotalRevenue { get; private set; }
    public decimal AverageOrderValue { get; private set; }
    public Dictionary<string, int> OrdersPerStatus { get; private set; } = new();
    public DateTime? LastRefreshed { get; private set; }

    public void Refresh(IEnumerable<Order> orders)
    {
        lock (_lock)
        {
            var list = orders.ToList();

            TotalOrders = list.Count;
            TotalRevenue = list.Sum(order => order.TotalAmount);
            AverageOrderValue = TotalOrders > 0 ? TotalRevenue / TotalOrders : 0m;
            OrdersPerStatus = list
                .GroupBy(order => order.Status.ToString())
                .ToDictionary(group => group.Key, group => group.Count());
            LastRefreshed = DateTime.UtcNow;
        }
    }

    public bool IsStale(TimeSpan maxAge)
    {
        return LastRefreshed is null || DateTime.UtcNow - LastRefreshed.Value > maxAge;
    }
}
