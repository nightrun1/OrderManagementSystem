namespace OrderManagementSystem.Lab.Lab4.Composite;

public interface ICartItem
{
    int Id { get; }
    string Name { get; }
    decimal GetPrice();
    int GetTotalQuantity();
    IEnumerable<ICartItem> GetChildren();
    void DisplayInfo(int indent = 0);
}
