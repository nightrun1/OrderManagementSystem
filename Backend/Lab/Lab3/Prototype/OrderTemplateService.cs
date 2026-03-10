using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Lab.Lab3.Prototype;

public class OrderTemplateService(AppDbContext context)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<OrderTemplate> CreateTemplateFromOrderAsync(int orderId, string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new InvalidOperationException("Template name is required.");
        }

        var order = await context.Orders
            .Include(o => o.Items)
            .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(o => o.Id == orderId);

        if (order is null)
        {
            throw new InvalidOperationException("Order was not found.");
        }

        return new OrderTemplate
        {
            Name = name.Trim(),
            CreatedByUserId = order.UserId,
            ShippingAddress = order.ShippingAddress,
            Items = order.Items.Select(item => new OrderItemSnapshot
            {
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice
            }).ToList(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task SaveTemplateAsync(OrderTemplate template)
    {
        var entity = new OrderTemplateEntity
        {
            Name = template.Name,
            UserId = template.CreatedByUserId,
            TemplateJson = JsonSerializer.Serialize(template, JsonOptions),
            CreatedAt = DateTime.UtcNow
        };

        context.OrderTemplates.Add(entity);
        await context.SaveChangesAsync();

        template.Id = entity.Id;
        template.CreatedAt = entity.CreatedAt;
    }

    public async Task<List<OrderTemplate>> GetUserTemplatesAsync(int userId)
    {
        var entities = await context.OrderTemplates
            .Where(template => template.UserId == userId)
            .OrderByDescending(template => template.CreatedAt)
            .ToListAsync();

        var templates = new List<OrderTemplate>();

        foreach (var entity in entities)
        {
            var template = DeserializeTemplate(entity);
            if (template is null)
            {
                continue;
            }

            templates.Add(template);
        }

        return templates;
    }

    public async Task<OrderTemplate?> GetTemplateAsync(int templateId)
    {
        var entity = await context.OrderTemplates.FirstOrDefaultAsync(template => template.Id == templateId);
        if (entity is null)
        {
            return null;
        }

        return DeserializeTemplate(entity);
    }

    public async Task<Order> CloneAsNewOrderAsync(int templateId)
    {
        var template = await GetTemplateAsync(templateId);
        if (template is null)
        {
            throw new InvalidOperationException("Template was not found.");
        }

        var clone = (OrderTemplate)template.DeepClone();

        if (clone.Items.Count == 0)
        {
            throw new InvalidOperationException("Template does not contain any items.");
        }

        var productIds = clone.Items.Select(item => item.ProductId).Distinct().ToList();
        var products = await context.Products
            .Where(product => product.IsActive && productIds.Contains(product.Id))
            .ToDictionaryAsync(product => product.Id);

        var orderItems = new List<OrderItem>();
        var totalAmount = 0m;

        foreach (var item in clone.Items)
        {
            if (!products.TryGetValue(item.ProductId, out var product))
            {
                throw new InvalidOperationException($"Product {item.ProductId} is no longer available.");
            }

            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException($"Invalid quantity for product {item.ProductId}.");
            }

            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");
            }

            product.Stock -= item.Quantity;
            totalAmount += product.Price * item.Quantity;

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            UserId = clone.CreatedByUserId,
            ShippingAddress = clone.ShippingAddress,
            Status = OrderStatus.Pending,
            TotalAmount = totalAmount,
            Items = orderItems
        };

        context.Orders.Add(order);
        await context.SaveChangesAsync();

        return await context.Orders
            .Include(o => o.Items)
            .ThenInclude(item => item.Product)
            .FirstAsync(o => o.Id == order.Id);
    }

    public async Task<bool> DeleteTemplateAsync(int templateId, int? userId = null)
    {
        var query = context.OrderTemplates.Where(template => template.Id == templateId);

        if (userId is not null)
        {
            query = query.Where(template => template.UserId == userId.Value);
        }

        var entity = await query.FirstOrDefaultAsync();
        if (entity is null)
        {
            return false;
        }

        context.OrderTemplates.Remove(entity);
        await context.SaveChangesAsync();
        return true;
    }

    private static OrderTemplate? DeserializeTemplate(OrderTemplateEntity entity)
    {
        if (string.IsNullOrWhiteSpace(entity.TemplateJson))
        {
            return null;
        }

        try
        {
            var template = JsonSerializer.Deserialize<OrderTemplate>(entity.TemplateJson, JsonOptions);
            if (template is null)
            {
                return null;
            }

            template.Id = entity.Id;
            template.Name = string.IsNullOrWhiteSpace(template.Name) ? entity.Name : template.Name;
            template.CreatedByUserId = entity.UserId;
            template.CreatedAt = entity.CreatedAt;
            return template;
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
