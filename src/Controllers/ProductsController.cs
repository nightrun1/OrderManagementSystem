using Microsoft.AspNetCore.Mvc;
using OrderManagementSystem.Interfaces.Services;

namespace OrderManagementSystem.Controllers;

// TODO: Implement Products API controller (REST endpoints)
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    // TODO: IProductService field (injected)
    // TODO: Constructor with service injection (DIP)
    
    // TODO: GET api/products - Get all products endpoint
    [HttpGet]
    public Task<IActionResult> GetAllProducts()
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/products/{id} - Get product by id endpoint
    [HttpGet("{id}")]
    public Task<IActionResult> GetProduct(int id)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/products/category/{category} - Get products by category endpoint
    [HttpGet("category/{category}")]
    public Task<IActionResult> GetProductsByCategory(int category)
    {
        throw new NotImplementedException();
    }
    
    // TODO: GET api/products/search - Search products endpoint
    [HttpGet("search")]
    public Task<IActionResult> SearchProducts(string term)
    {
        throw new NotImplementedException();
    }
    
    // TODO: POST api/products - Create product endpoint
    [HttpPost]
    public Task<IActionResult> CreateProduct()
    {
        throw new NotImplementedException();
    }
    
    // TODO: PUT api/products/{id} - Update product endpoint
    [HttpPut("{id}")]
    public Task<IActionResult> UpdateProduct(int id)
    {
        throw new NotImplementedException();
    }
}
