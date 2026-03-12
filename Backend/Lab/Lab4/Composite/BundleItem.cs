namespace OrderManagementSystem.Lab.Lab4.Composite;

public class BundleItem(int bundleId, string name, decimal discountPercent) : ICartItem
{
    private readonly List<ICartItem> _children = [];

    public int Id => bundleId;
    public string Name => name;
    public decimal DiscountPercent => discountPercent;

    public void Add(ICartItem item) => _children.Add(item);
    public void Remove(ICartItem item) => _children.Remove(item);

    public decimal GetPrice()
    {
        var subtotal = _children.Sum(c => c.GetPrice());
        return Math.Round(subtotal * (1 - DiscountPercent / 100), 2);
    }

    public int GetTotalQuantity() => _children.Sum(c => c.GetTotalQuantity());

    public IEnumerable<ICartItem> GetChildren() => _children;

    public void DisplayInfo(int indent = 0)
    {
        var spaces = new string(' ', indent);
        Console.WriteLine($"{spaces}[Bundle] {Name} (-{DiscountPercent}%) = {GetPrice()} lei");
        foreach (var child in _children)
            child.DisplayInfo(indent + 2);
    }
}
