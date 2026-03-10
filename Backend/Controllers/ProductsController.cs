using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.DTOs.Products;
using OrderManagementSystem.Interfaces;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Controllers;

[ApiController]
[Route("api/products")]
public class ProductsController(IProductRepository productRepository) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var products = (await productRepository.GetAllAsync())
            .Where(p => p.IsActive)
            .Select(MapToDto)
            .ToList();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ProductDto>> GetById(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null || !product.IsActive)
        {
            return NotFound(new { message = "Product was not found." });
        }

        return Ok(MapToDto(product));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Create(CreateProductRequest request)
    {
        if (await productRepository.GetBySkuAsync(request.SKU.Trim()) is not null)
        {
            return BadRequest(new { message = "SKU already exists." });
        }

        var product = new Product
        {
            Name = request.Name.Trim(),
            SKU = request.SKU.Trim(),
            Description = request.Description.Trim(),
            Category = request.Category.Trim(),
            Price = request.Price,
            Stock = request.Stock,
            IsActive = true
        };

        await productRepository.AddAsync(product);

        return CreatedAtAction(nameof(GetById), new { id = product.Id }, MapToDto(product));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> Update(int id, CreateProductRequest request)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound(new { message = "Product was not found." });
        }

        var existingBySku = await productRepository.GetBySkuAsync(request.SKU.Trim());
        if (existingBySku is not null && existingBySku.Id != id)
        {
            return BadRequest(new { message = "SKU already exists." });
        }

        product.Name = request.Name.Trim();
        product.SKU = request.SKU.Trim();
        product.Description = request.Description.Trim();
        product.Category = request.Category.Trim();
        product.Price = request.Price;
        product.Stock = request.Stock;

        await productRepository.UpdateAsync(product);

        return Ok(MapToDto(product));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var product = await productRepository.GetByIdAsync(id);
        if (product is null)
        {
            return NotFound(new { message = "Product was not found." });
        }

        product.IsActive = false;
        await productRepository.UpdateAsync(product);

        return NoContent();
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto(product.Id, product.Name, product.SKU, product.Price, product.Stock, product.Category);
    }
}
