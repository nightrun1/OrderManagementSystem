namespace OrderManagementSystem.Lab.Lab4.Facade;

public class PlaceOrderResult
{
    public bool Success { get; set; }
    public int? OrderId { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorStep { get; set; }
    public decimal TotalCharged { get; set; }
    public string? TrackingNumber { get; set; }
    public string ReceiptText { get; set; } = string.Empty;
}
