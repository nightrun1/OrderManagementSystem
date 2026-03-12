namespace OrderManagementSystem.Lab.Lab4.Composite;

public class ShoppingCart(int userId)
{
    private readonly List<ICartItem> _items = [];

    public int UserId => userId;

    public void AddItem(ICartItem item) => _items.Add(item);

    public void RemoveItem(int itemId) => _items.RemoveAll(i => i.Id == itemId);

    public decimal GetTotal() => _items.Sum(i => i.GetPrice());

    public int GetItemCount() => _items.Sum(i => i.GetTotalQuantity());

    public IEnumerable<ICartItem> GetItems() => _items;

    public List<CartItemDto> Flatten()
    {
        var result = new List<CartItemDto>();
        foreach (var item in _items)
            FlattenRecursive(item, 0, result);
        return result;
    }

    private static void FlattenRecursive(ICartItem item, int depth, List<CartItemDto> result)
    {
        var isBundle = item.GetChildren().Any();
        result.Add(new CartItemDto(item.Id, item.Name, item.GetPrice(), item.GetTotalQuantity(), isBundle, depth));

        foreach (var child in item.GetChildren())
            FlattenRecursive(child, depth + 1, result);
    }
}
