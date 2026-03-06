using OrderManagementSystem.API.Interfaces;

namespace OrderManagementSystem.API.Models;

public enum OrderStatus
{
    Pending = 0,
    Processing = 1,
    Shipped = 2,
    Delivered = 3,
    Cancelled = 4
}

public sealed class Order : IOrder
{
    private readonly List<OrderItem> _orderItems = new();

    private Order()
    {
    }

    public Order(int customerId)
    {
        if (customerId <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(customerId), "CustomerId must be greater than zero.");
        }

        CustomerId = customerId;
        CreatedAt = DateTime.UtcNow;
        Status = OrderStatus.Pending;
    }

    public int Id { get; private set; }

    public int CustomerId { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public OrderStatus Status { get; private set; }

    public IReadOnlyCollection<OrderItem> OrderItems => _orderItems.AsReadOnly();

    public decimal TotalAmount => _orderItems.Sum(item => item.Subtotal);

    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);

        if (Status is OrderStatus.Shipped or OrderStatus.Delivered or OrderStatus.Cancelled)
        {
            throw new InvalidOperationException("Items cannot be modified when the order is finalized.");
        }

        item.SetOrderId(Id);
        _orderItems.Add(item);
    }

    public void UpdateStatus(OrderStatus newStatus)
    {
        if (!CanTransitionTo(newStatus))
        {
            throw new InvalidOperationException($"Cannot transition order status from {Status} to {newStatus}.");
        }

        Status = newStatus;
    }

    private bool CanTransitionTo(OrderStatus newStatus)
    {
        if (Status == newStatus)
        {
            return true;
        }

        return (Status, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Processing) => true,
            (OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Processing, OrderStatus.Shipped) => true,
            (OrderStatus.Processing, OrderStatus.Cancelled) => true,
            (OrderStatus.Shipped, OrderStatus.Delivered) => true,
            _ => false
        };
    }
}