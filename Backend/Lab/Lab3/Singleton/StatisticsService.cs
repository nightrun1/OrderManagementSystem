using OrderManagementSystem.Interfaces;

namespace OrderManagementSystem.Lab.Lab3.Singleton;

public class StatisticsService(IOrderRepository orderRepository)
{
    private static readonly TimeSpan CacheMaxAge = TimeSpan.FromMinutes(5);

    public async Task<OrderStatisticsCache> GetStatisticsAsync()
    {
        var cache = OrderStatisticsCache.Instance;

        if (cache.IsStale(CacheMaxAge))
        {
            var orders = await orderRepository.GetAllAsync();
            cache.Refresh(orders);
        }

        return cache;
    }

    public async Task<OrderStatisticsCache> ForceRefreshAsync()
    {
        var orders = await orderRepository.GetAllAsync();
        var cache = OrderStatisticsCache.Instance;
        cache.Refresh(orders);

        return cache;
    }
}
