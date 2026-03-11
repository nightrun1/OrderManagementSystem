namespace OrderManagementSystem.Lab.Lab4.Adapter.FanCourier;

public class FanCourierClient
{
    public FanCourierRate CalculateRate(string judetExpeditor, string judetDestinatie, float greutate)
    {
        var basePret = greutate * 5.5f;
        var sameJudet = string.Equals(judetExpeditor, judetDestinatie, StringComparison.OrdinalIgnoreCase);

        return new FanCourierRate
        {
            Pret = sameJudet ? basePret : basePret + 8f,
            ZileEstimate = sameJudet ? 1 : 3,
            TipServici = sameJudet ? "Standard Local" : "Standard National"
        };
    }

    public string GenerateAWB(FanCourierShipmentData data)
    {
        return $"FC-{Guid.NewGuid():N}"[..16].ToUpperInvariant();
    }

    public FanCourierStatus GetAWBStatus(string awb)
    {
        return new FanCourierStatus
        {
            AWB = awb,
            Stare = "In tranzit",
            UltimaLocatie = "Depozit Central Bucuresti",
            DataActualizare = DateTime.UtcNow
        };
    }
}

public class FanCourierRate
{
    public float Pret { get; set; }
    public int ZileEstimate { get; set; }
    public string TipServici { get; set; } = string.Empty;
}

public class FanCourierShipmentData
{
    public string NumeDestinatar { get; set; } = string.Empty;
    public string AdresaDestinatie { get; set; } = string.Empty;
    public string JudetDestinatie { get; set; } = string.Empty;
    public float Greutate { get; set; }
}

public class FanCourierStatus
{
    public string AWB { get; set; } = string.Empty;
    public string Stare { get; set; } = string.Empty;
    public string UltimaLocatie { get; set; } = string.Empty;
    public DateTime DataActualizare { get; set; }
}
