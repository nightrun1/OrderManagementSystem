namespace OrderManagementSystem.Lab.Lab4.Adapter.DPD;

public class DpdApiClient
{
    public DpdPriceResponse QueryPrice(DpdPriceRequest request)
    {
        var basePrice = request.WeightKg * 4.2m;
        var sameCityDiscount = string.Equals(request.OriginCity, request.DestCity, StringComparison.OrdinalIgnoreCase)
            ? 0.8m : 1.0m;

        var net = basePrice * sameCityDiscount;
        return new DpdPriceResponse
        {
            NetPrice = Math.Round(net, 2),
            GrossPrice = Math.Round(net * 1.19m, 2),
            DeliveryDays = sameCityDiscount < 1 ? 1 : 2
        };
    }

    public DpdParcelResponse RegisterParcel(DpdParcelRequest request)
    {
        return new DpdParcelResponse
        {
            ParcelId = $"DPD-{Guid.NewGuid():N}"[..16].ToUpperInvariant(),
            Label = $"Label-{request.RecipientName}",
            Success = true
        };
    }

    public DpdTrackResponse TrackParcel(string parcelId)
    {
        return new DpdTrackResponse
        {
            ParcelId = parcelId,
            CurrentStatus = "In delivery",
            CurrentDepot = "DPD Hub Chisinau",
            Timestamp = DateTime.UtcNow
        };
    }
}

public class DpdPriceRequest
{
    public string OriginCountry { get; set; } = "RO";
    public string OriginCity { get; set; } = string.Empty;
    public string DestCity { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
}

public class DpdPriceResponse
{
    public decimal NetPrice { get; set; }
    public decimal GrossPrice { get; set; }
    public int DeliveryDays { get; set; }
}

public class DpdParcelRequest
{
    public string RecipientName { get; set; } = string.Empty;
    public string DestAddress { get; set; } = string.Empty;
    public string DestCity { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
}

public class DpdParcelResponse
{
    public string ParcelId { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public bool Success { get; set; }
}

public class DpdTrackResponse
{
    public string ParcelId { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public string CurrentDepot { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}
