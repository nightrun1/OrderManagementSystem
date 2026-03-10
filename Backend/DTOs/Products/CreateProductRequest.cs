namespace OrderManagementSystem.DTOs.Products;

public record CreateProductRequest(
    string Name,
    string SKU,
    string Description,
    string Category,
    decimal Price,
    int Stock);
