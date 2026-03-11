using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab4.Adapter.FanCourier;

public class FanCourierAdapter(FanCourierClient client) : IShippingProvider
{
    public string ProviderName => "FanCourier";

    public Task<ShippingQuote> GetQuoteAsync(string fromCity, string toCity, double weightKg)
    {
        var rate = client.CalculateRate(fromCity, toCity, (float)weightKg);
        var quote = new ShippingQuote("FanCourier", (decimal)rate.Pret, rate.ZileEstimate, rate.TipServici);
        return Task.FromResult(quote);
    }

    public Task<string> CreateShipmentAsync(Order order)
    {
        var data = new FanCourierShipmentData
        {
            NumeDestinatar = $"User #{order.UserId}",
            AdresaDestinatie = order.ShippingAddress,
            JudetDestinatie = order.ShippingAddress.Split(',').LastOrDefault()?.Trim() ?? "Bucuresti",
            Greutate = 1.0f
        };
        var awb = client.GenerateAWB(data);
        return Task.FromResult(awb);
    }

    public Task<ShippingTrackingInfo> TrackShipmentAsync(string trackingNumber)
    {
        var status = client.GetAWBStatus(trackingNumber);
        var info = new ShippingTrackingInfo(status.AWB, status.Stare, status.UltimaLocatie, status.DataActualizare);
        return Task.FromResult(info);
    }
}
