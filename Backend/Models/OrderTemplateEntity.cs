namespace OrderManagementSystem.Models;

public class OrderTemplateEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public string TemplateJson { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
