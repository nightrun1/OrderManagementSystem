namespace OrderManagementSystem.DTOs.Products;

public record ProductDto(int Id, string Name, string SKU, decimal Price, int Stock, string Category);
