namespace OrderManagementSystem.Lab.Lab3.Prototype;

public class OrderTemplate : ICloneableOrder
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CreatedByUserId { get; set; }
    public string ShippingAddress { get; set; } = string.Empty;
    public List<OrderItemSnapshot> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICloneableOrder DeepClone()
    {
        return new OrderTemplate
        {
            Id = 0,
            Name = $"{Name} (copie)",
            CreatedByUserId = CreatedByUserId,
            ShippingAddress = ShippingAddress,
            Items = Items.Select(item => new OrderItemSnapshot
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),
            CreatedAt = DateTime.UtcNow
        };
    }
}
