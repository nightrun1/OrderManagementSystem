using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab4.Adapter;

public class ShippingService(IEnumerable<IShippingProvider> providers)
{
    public async Task<List<ShippingQuote>> GetAllQuotesAsync(string fromCity, string toCity, double weightKg)
    {
        var tasks = providers.Select(p => p.GetQuoteAsync(fromCity, toCity, weightKg));
        return (await Task.WhenAll(tasks)).ToList();
    }

    public async Task<string> CreateShipmentAsync(string providerName, Order order)
    {
        var provider = providers.FirstOrDefault(p =>
            p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Shipping provider '{providerName}' not found.");

        return await provider.CreateShipmentAsync(order);
    }

    public async Task<ShippingTrackingInfo> TrackAsync(string providerName, string trackingNumber)
    {
        var provider = providers.FirstOrDefault(p =>
            p.ProviderName.Equals(providerName, StringComparison.OrdinalIgnoreCase))
            ?? throw new ArgumentException($"Shipping provider '{providerName}' not found.");

        return await provider.TrackShipmentAsync(trackingNumber);
    }
}
