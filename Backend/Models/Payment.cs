namespace OrderManagementSystem.Models;

public class Payment
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public decimal Amount { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string Status { get; set; } = "Pending";
    public string TransactionId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
