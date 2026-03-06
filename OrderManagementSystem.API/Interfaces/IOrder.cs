using OrderManagementSystem.API.Models;

namespace OrderManagementSystem.API.Interfaces;

public interface IOrder
{
    int Id { get; }

    int CustomerId { get; }

    DateTime CreatedAt { get; }

    OrderStatus Status { get; }

    IReadOnlyCollection<OrderItem> OrderItems { get; }

    decimal TotalAmount { get; }

    void AddItem(OrderItem item);

    void UpdateStatus(OrderStatus newStatus);
}