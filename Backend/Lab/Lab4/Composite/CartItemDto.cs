namespace OrderManagementSystem.Lab.Lab4.Composite;

public record CartItemDto(int Id, string Name, decimal Price, int Quantity, bool IsBundle, int Depth);
