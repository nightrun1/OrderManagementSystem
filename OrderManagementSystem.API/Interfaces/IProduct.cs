namespace OrderManagementSystem.API.Interfaces;

public interface IProduct
{
    int Id { get; }

    string Name { get; }

    string Description { get; }

    decimal Price { get; }

    int Stock { get; }

    int CategoryId { get; }

    void UpdateDetails(string name, string? description, int categoryId);

    void UpdatePrice(decimal price);

    void IncreaseStock(int amount);

    void DecreaseStock(int amount);
}