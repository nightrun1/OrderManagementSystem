using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Interfaces;

namespace OrderManagementSystem.Lab.Lab3.Builder;

public class CustomOrderBuilder(IProductRepository productRepository) : ICustomOrderBuilder
{
    private int _userId;
    private readonly List<OrderItemRequest> _items = [];
    private string _shippingAddress = string.Empty;
    private string? _discountCode;
    private bool _isPriority;
    private string? _customerNote;
    private string _deliveryOption = "standard";

    public ICustomOrderBuilder ForUser(int userId)
    {
        _userId = userId;
        return this;
    }

    public ICustomOrderBuilder AddItem(int productId, int quantity)
    {
        _items.Add(new OrderItemRequest(productId, quantity));
        return this;
    }

    public ICustomOrderBuilder ShipTo(string address)
    {
        _shippingAddress = address.Trim();
        return this;
    }

    public ICustomOrderBuilder WithDiscountCode(string code)
    {
        _discountCode = code.Trim();
        return this;
    }

    public ICustomOrderBuilder AsPriority()
    {
        _isPriority = true;
        return this;
    }

    public ICustomOrderBuilder WithNote(string note)
    {
        _customerNote = note.Trim();
        return this;
    }

    public ICustomOrderBuilder WithDeliveryOption(string option)
    {
        _deliveryOption = option.Trim().ToLowerInvariant();
        return this;
    }

    public CustomOrderDto Build()
    {
        if (_userId <= 0)
        {
            throw new InvalidOperationException("A valid user is required.");
        }

        if (_items.Count == 0)
        {
            throw new InvalidOperationException("At least one item is required.");
        }

        if (string.IsNullOrWhiteSpace(_shippingAddress))
        {
            throw new InvalidOperationException("Shipping address is required.");
        }

        var normalizedDelivery = NormalizeDeliveryOption(_deliveryOption);
        var subtotal = 0m;

        foreach (var item in _items)
        {
            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException($"Invalid quantity for product {item.ProductId}.");
            }

            var product = productRepository.GetByIdAsync(item.ProductId).GetAwaiter().GetResult();
            if (product is null || !product.IsActive)
            {
                throw new InvalidOperationException($"Product {item.ProductId} was not found.");
            }

            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");
            }

            subtotal += product.Price * item.Quantity;
        }

        var discountAmount = CalculateDiscount(subtotal, _discountCode);
        var shippingCost = CalculateShippingCost(normalizedDelivery, _isPriority);
        var finalTotal = subtotal - discountAmount + shippingCost;

        var result = new CustomOrderDto
        {
            UserId = _userId,
            Items = _items
                .Select(item => new OrderItemRequest(item.ProductId, item.Quantity))
                .ToList(),
            ShippingAddress = _shippingAddress,
            DiscountCode = _discountCode,
            IsPriority = _isPriority,
            CustomerNote = _customerNote,
            DeliveryOption = normalizedDelivery,
            ShippingCost = shippingCost,
            DiscountAmount = discountAmount,
            FinalTotal = finalTotal < 0 ? 0 : finalTotal
        };

        Reset();
        return result;
    }

    private static string NormalizeDeliveryOption(string option)
    {
        return option switch
        {
            "standard" => "standard",
            "express" => "express",
            "pickup" => "pickup",
            _ => throw new InvalidOperationException("Delivery option must be standard, express or pickup.")
        };
    }

    private static decimal CalculateDiscount(decimal subtotal, string? discountCode)
    {
        if (string.IsNullOrWhiteSpace(discountCode))
        {
            return 0m;
        }

        var normalizedCode = discountCode.Trim().ToUpperInvariant();

        var discount = normalizedCode switch
        {
            "SAVE10" => subtotal * 0.10m,
            "SAVE50" => 50m,
            _ => 0m
        };

        return discount > subtotal ? subtotal : discount;
    }

    private static decimal CalculateShippingCost(string deliveryOption, bool isPriority)
    {
        var shippingCost = deliveryOption switch
        {
            "express" => 45m,
            "pickup" => 0m,
            _ => 15m
        };

        if (isPriority)
        {
            shippingCost += 20m;
        }

        return shippingCost;
    }

    private void Reset()
    {
        _userId = 0;
        _items.Clear();
        _shippingAddress = string.Empty;
        _discountCode = null;
        _isPriority = false;
        _customerNote = null;
        _deliveryOption = "standard";
    }
}
