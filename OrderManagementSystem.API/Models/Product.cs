using OrderManagementSystem.API.Interfaces;

namespace OrderManagementSystem.API.Models;

public sealed class Product : IProduct
{
    private Product()
    {
    }

    public Product(string name, string? description, decimal price, int stock, int categoryId)
    {
        UpdateDetails(name, description, categoryId);
        UpdatePrice(price);
        SetStock(stock);
    }

    public int Id { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public decimal Price { get; private set; }

    public int Stock { get; private set; }

    public int CategoryId { get; private set; }

    public void UpdateDetails(string name, string? description, int categoryId)
    {
        Name = ValidateRequiredText(name, nameof(name));
        Description = description?.Trim() ?? string.Empty;

        if (categoryId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(categoryId), "CategoryId must be greater than zero.");
        }

        CategoryId = categoryId;
    }

    public void UpdatePrice(decimal price)
    {
        if (price < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        }

        Price = price;
    }

    public void IncreaseStock(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Increase amount must be greater than zero.");
        }

        Stock += amount;
    }

    public void DecreaseStock(int amount)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Decrease amount must be greater than zero.");
        }

        if (amount > Stock)
        {
            throw new InvalidOperationException("Insufficient stock.");
        }

        Stock -= amount;
    }

    private void SetStock(int stock)
    {
        if (stock < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(stock), "Stock cannot be negative.");
        }

        Stock = stock;
    }

    private static string ValidateRequiredText(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value is required.", parameterName);
        }

        return value.Trim();
    }
}