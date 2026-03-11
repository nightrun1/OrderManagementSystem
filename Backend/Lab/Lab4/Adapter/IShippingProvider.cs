namespace OrderManagementSystem.Lab.Lab4.Adapter;

public interface IShippingProvider
{
    string ProviderName { get; }
    Task<ShippingQuote> GetQuoteAsync(string fromCity, string toCity, double weightKg);
    Task<string> CreateShipmentAsync(Models.Order order);
    Task<ShippingTrackingInfo> TrackShipmentAsync(string trackingNumber);
}
