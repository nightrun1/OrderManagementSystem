using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Lab.Lab4.Composite;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/cart")]
[Authorize]
public class CartController(IProductRepository productRepository) : ControllerBase
{
    private static readonly ConcurrentDictionary<int, ShoppingCart> Carts = new();

    [HttpGet]
    public ActionResult<object> GetCart()
    {
        var cart = GetOrCreateCart();
        var items = cart.Flatten();
        return Ok(new { items, total = cart.GetTotal(), itemCount = cart.GetItemCount() });
    }

    [HttpPost("items")]
    public async Task<ActionResult<object>> AddItem([FromBody] AddCartItemRequest request)
    {
        var product = await productRepository.GetByIdAsync(request.ProductId);
        if (product is null)
            return NotFound(new { message = $"Produsul #{request.ProductId} nu a fost gasit." });

        var cart = GetOrCreateCart();
        var item = new SimpleProductItem(product.Id, product.Name, product.Price, request.Quantity);
        cart.AddItem(item);

        return Ok(new { message = "Produs adaugat in cos.", items = cart.Flatten(), total = cart.GetTotal() });
    }

    [HttpPost("bundles")]
    public async Task<ActionResult<object>> AddBundle([FromBody] AddBundleRequest request)
    {
        var bundle = new BundleItem(request.BundleId, request.BundleName, request.DiscountPercent);

        foreach (var component in request.Products)
        {
            var product = await productRepository.GetByIdAsync(component.ProductId);
            if (product is null)
                return NotFound(new { message = $"Produsul #{component.ProductId} din bundle nu a fost gasit." });

            bundle.Add(new SimpleProductItem(product.Id, product.Name, product.Price, component.Quantity));
        }

        var cart = GetOrCreateCart();
        cart.AddItem(bundle);

        return Ok(new { message = "Bundle adaugat in cos.", items = cart.Flatten(), total = cart.GetTotal() });
    }

    [HttpDelete("items/{itemId:int}")]
    public ActionResult<object> RemoveItem(int itemId)
    {
        var cart = GetOrCreateCart();
        cart.RemoveItem(itemId);
        return Ok(new { message = "Item sters din cos.", items = cart.Flatten(), total = cart.GetTotal() });
    }

    [HttpGet("total")]
    public ActionResult<object> GetTotal()
    {
        var cart = GetOrCreateCart();
        return Ok(new { subtotal = cart.GetTotal(), itemCount = cart.GetItemCount(), items = cart.Flatten() });
    }

    [HttpDelete]
    public ActionResult<object> ClearCart()
    {
        var userId = GetCurrentUserId();
        Carts.TryRemove(userId, out _);
        return Ok(new { message = "Cosul a fost golit." });
    }

    private ShoppingCart GetOrCreateCart()
    {
        var userId = GetCurrentUserId();
        return Carts.GetOrAdd(userId, id => new ShoppingCart(id));
    }

    private int GetCurrentUserId()
    {
        var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(userIdValue, out var userId) ? userId : 0;
    }
}

public record AddCartItemRequest(int ProductId, int Quantity);
public record AddBundleRequest(int BundleId, string BundleName, decimal DiscountPercent, List<BundleProductRequest> Products);
public record BundleProductRequest(int ProductId, int Quantity);
