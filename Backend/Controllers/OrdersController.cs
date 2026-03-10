using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs.Orders;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Lab.Lab2.FactoryMethod;
using OrderManagementSystem.Lab.Lab3.Builder;
using OrderManagementSystem.Lab.Lab3.Prototype;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/orders")]
[Authorize]
public class OrdersController(
    IOrderRepository orderRepository,
    IProductRepository productRepository,
    OrderTemplateService orderTemplateService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetMyOrders()
    {
        var currentUserId = GetCurrentUserId();
        var orders = await orderRepository.GetByUserIdAsync(currentUserId);

        return Ok(orders.Select(order => MapToDto(order)).ToList());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await orderRepository.GetWithItemsAsync(id);
        if (order is null)
        {
            return NotFound(new { message = "Order was not found." });
        }

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        if (!isAdmin && order.UserId != currentUserId)
        {
            return StatusCode(403, new { message = "You do not have access to this order." });
        }

        return Ok(MapToDto(order));
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest(new { message = "Order must contain at least one item." });
        }

        OrderCreator creator;
        try
        {
            creator = OrderCreatorFactory.GetCreator(request.OrderType);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        var productMap = new Dictionary<int, Product>();
        foreach (var item in request.Items)
        {
            if (item.Quantity <= 0)
            {
                return BadRequest(new { message = $"Invalid quantity for product {item.ProductId}." });
            }

            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product is null || !product.IsActive)
            {
                return NotFound(new { message = $"Product {item.ProductId} was not found." });
            }

            if (product.Stock < item.Quantity)
            {
                return BadRequest(new { message = $"Insufficient stock for product {item.ProductId}." });
            }

            productMap[item.ProductId] = product;
        }

        decimal totalAmount = 0;
        var orderItems = new List<OrderItem>();

        foreach (var item in request.Items)
        {
            var product = productMap[item.ProductId];
            totalAmount += product.Price * item.Quantity;
            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });

            product.Stock -= item.Quantity;
            await productRepository.UpdateAsync(product);
        }

        OrderCreationResult creationResult;
        try
        {
            creationResult = creator.ProcessOrder(request, GetCurrentUserId(), totalAmount);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }

        var order = new Order
        {
            UserId = creationResult.Order.UserId,
            ShippingAddress = creationResult.Order.ShippingAddress,
            Status = creationResult.Order.Status,
            TotalAmount = creationResult.Order.TotalAmount,
            Items = orderItems
        };

        await orderRepository.AddAsync(order);

        var savedOrder = await orderRepository.GetWithItemsAsync(order.Id) ?? order;
        return Ok(MapToDto(savedOrder, creationResult.Order.OrderType, creationResult.ShippingCost));
    }

    [HttpPost("custom")]
    public async Task<ActionResult<CustomOrderResultDto>> CreateCustom(CreateCustomOrderRequest request)
    {
        if (request.Items.Count == 0)
        {
            return BadRequest(new { message = "Order must contain at least one item." });
        }

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());
        var targetUserId = isAdmin ? request.UserId : currentUserId;

        var builder = new CustomOrderBuilder(productRepository);
        CustomOrderDto customOrder;

        try
        {
            if (request.UseDirectorPreset)
            {
                var preset = request.PresetType?.Trim().ToLowerInvariant();
                var director = new OrderDirector(builder);

                customOrder = preset switch
                {
                    "quick" => BuildQuickPreset(director, targetUserId, request),
                    "priority" => director.BuildPriorityOrder(targetUserId, request.Items, request.ShippingAddress),
                    _ => throw new InvalidOperationException("PresetType must be quick or priority.")
                };
            }
            else
            {
                var fluentBuilder = builder
                    .ForUser(targetUserId)
                    .ShipTo(request.ShippingAddress)
                    .WithDeliveryOption(request.DeliveryOption);

                foreach (var item in request.Items)
                {
                    fluentBuilder.AddItem(item.ProductId, item.Quantity);
                }

                if (!string.IsNullOrWhiteSpace(request.DiscountCode))
                {
                    fluentBuilder.WithDiscountCode(request.DiscountCode);
                }

                if (request.IsPriority)
                {
                    fluentBuilder.AsPriority();
                }

                if (!string.IsNullOrWhiteSpace(request.Note))
                {
                    fluentBuilder.WithNote(request.Note);
                }

                customOrder = fluentBuilder.Build();
            }

            var savedOrder = await SaveCustomOrderAsync(customOrder);

            return Ok(new CustomOrderResultDto(
                savedOrder.Id,
                savedOrder.Status,
                customOrder.FinalTotal,
                customOrder.ShippingCost,
                customOrder.DiscountAmount,
                customOrder.DeliveryOption,
                customOrder.IsPriority,
                customOrder.ShippingAddress,
                savedOrder.CreatedAt,
                savedOrder.Items
                    .Select(item => new OrderItemDto(item.ProductId, item.Product.Name, item.Quantity, item.UnitPrice))
                    .ToList(),
                customOrder.CustomerNote));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPost("{id:int}/save-as-template")]
    public async Task<ActionResult<OrderTemplateDto>> SaveAsTemplate(int id, SaveOrderTemplateRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest(new { message = "Template name is required." });
        }

        var order = await orderRepository.GetWithItemsAsync(id);
        if (order is null)
        {
            return NotFound(new { message = "Order was not found." });
        }

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        if (!isAdmin && order.UserId != currentUserId)
        {
            return StatusCode(403, new { message = "You do not have access to this order." });
        }

        try
        {
            var template = await orderTemplateService.CreateTemplateFromOrderAsync(id, request.Name);
            await orderTemplateService.SaveTemplateAsync(template);
            return Ok(MapTemplateDto(template));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("templates")]
    public async Task<ActionResult<IEnumerable<OrderTemplateDto>>> GetTemplates()
    {
        var currentUserId = GetCurrentUserId();
        var templates = await orderTemplateService.GetUserTemplatesAsync(currentUserId);

        return Ok(templates.Select(MapTemplateDto).ToList());
    }

    [HttpPost("templates/{id:int}/clone")]
    public async Task<ActionResult<OrderDto>> CloneTemplate(int id)
    {
        var template = await orderTemplateService.GetTemplateAsync(id);
        if (template is null)
        {
            return NotFound(new { message = "Template was not found." });
        }

        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        if (!isAdmin && template.CreatedByUserId != currentUserId)
        {
            return StatusCode(403, new { message = "You do not have access to this template." });
        }

        try
        {
            var clonedOrder = await orderTemplateService.CloneAsNewOrderAsync(id);
            return Ok(MapToDto(clonedOrder, "TemplateClone", 0m));
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("templates/{id:int}")]
    public async Task<IActionResult> DeleteTemplate(int id)
    {
        var currentUserId = GetCurrentUserId();
        var isAdmin = User.IsInRole(UserRole.Admin.ToString());

        var deleted = await orderTemplateService.DeleteTemplateAsync(id, isAdmin ? null : currentUserId);
        if (!deleted)
        {
            return NotFound(new { message = "Template was not found." });
        }

        return NoContent();
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : 0;
    }

    private async Task<Order> SaveCustomOrderAsync(CustomOrderDto customOrder)
    {
        var orderItems = new List<OrderItem>();

        foreach (var item in customOrder.Items)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId);
            if (product is null || !product.IsActive)
            {
                throw new KeyNotFoundException($"Product {item.ProductId} was not found.");
            }

            if (product.Stock < item.Quantity)
            {
                throw new InvalidOperationException($"Insufficient stock for product {item.ProductId}.");
            }

            product.Stock -= item.Quantity;
            await productRepository.UpdateAsync(product);

            orderItems.Add(new OrderItem
            {
                ProductId = product.Id,
                Quantity = item.Quantity,
                UnitPrice = product.Price
            });
        }

        var order = new Order
        {
            UserId = customOrder.UserId,
            ShippingAddress = customOrder.ShippingAddress,
            Status = OrderStatus.Pending,
            TotalAmount = customOrder.FinalTotal,
            Items = orderItems
        };

        await orderRepository.AddAsync(order);

        return await orderRepository.GetWithItemsAsync(order.Id)
            ?? throw new InvalidOperationException("Order could not be loaded after creation.");
    }

    private static CustomOrderDto BuildQuickPreset(OrderDirector director, int userId, CreateCustomOrderRequest request)
    {
        var firstItem = request.Items.FirstOrDefault();
        if (firstItem is null)
        {
            throw new InvalidOperationException("Quick preset requires at least one item.");
        }

        return director.BuildQuickOrder(userId, firstItem.ProductId, firstItem.Quantity, request.ShippingAddress);
    }

    private static OrderTemplateDto MapTemplateDto(OrderTemplate template)
    {
        return new OrderTemplateDto(
            template.Id,
            template.Name,
            template.CreatedByUserId,
            template.ShippingAddress,
            template.CreatedAt,
            template.Items.Count,
            template.Items
                .Select(item => new OrderItemSnapshotDto(item.ProductId, item.ProductName, item.Quantity, item.UnitPrice))
                .ToList());
    }

    private static OrderDto MapToDto(Order order, string orderType = "Standard", decimal shippingCost = 15m)
    {
        return new OrderDto(
            order.Id,
            order.Status,
            order.TotalAmount,
            order.ShippingAddress,
            order.CreatedAt,
            order.Items
                .Select(i => new OrderItemDto(i.ProductId, i.Product.Name, i.Quantity, i.UnitPrice))
                .ToList(),
            orderType,
            shippingCost);
    }
}
