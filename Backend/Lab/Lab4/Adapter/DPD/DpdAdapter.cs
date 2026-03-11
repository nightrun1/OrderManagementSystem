using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab4.Adapter.DPD;

public class DpdAdapter(DpdApiClient client) : IShippingProvider
{
    public string ProviderName => "DPD";

    public Task<ShippingQuote> GetQuoteAsync(string fromCity, string toCity, double weightKg)
    {
        var request = new DpdPriceRequest
        {
            OriginCity = fromCity,
            DestCity = toCity,
            WeightKg = (decimal)weightKg
        };
        var response = client.QueryPrice(request);
        var quote = new ShippingQuote("DPD", response.GrossPrice, response.DeliveryDays, "DPD Standard");
        return Task.FromResult(quote);
    }

    public Task<string> CreateShipmentAsync(Order order)
    {
        var request = new DpdParcelRequest
        {
            RecipientName = $"User #{order.UserId}",
            DestAddress = order.ShippingAddress,
            DestCity = order.ShippingAddress.Split(',').FirstOrDefault()?.Trim() ?? "Chisinau",
            WeightKg = 1.0m
        };
        var response = client.RegisterParcel(request);
        return Task.FromResult(response.ParcelId);
    }

    public Task<ShippingTrackingInfo> TrackShipmentAsync(string trackingNumber)
    {
        var response = client.TrackParcel(trackingNumber);
        var info = new ShippingTrackingInfo(response.ParcelId, response.CurrentStatus, response.CurrentDepot, response.Timestamp);
        return Task.FromResult(info);
    }
}
