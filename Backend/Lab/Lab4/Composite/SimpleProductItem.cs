namespace OrderManagementSystem.Lab.Lab4.Composite;

public class SimpleProductItem(int productId, string name, decimal unitPrice, int quantity) : ICartItem
{
    public int Id => productId;
    public string Name => name;
    public decimal UnitPrice => unitPrice;
    public int Quantity => quantity;

    public decimal GetPrice() => UnitPrice * Quantity;

    public int GetTotalQuantity() => Quantity;

    public IEnumerable<ICartItem> GetChildren() => Enumerable.Empty<ICartItem>();

    public void DisplayInfo(int indent = 0)
    {
        var spaces = new string(' ', indent);
        Console.WriteLine($"{spaces}{Name} x{Quantity} = {GetPrice()} lei");
    }
}
