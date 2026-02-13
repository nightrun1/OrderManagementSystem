using Microsoft.EntityFrameworkCore;
using OrderManagementSystem.Models;

namespace OrderManagementSystem.Data;

// TODO: Implement DbContext for Order Management System (EF Core configuration)
public class OrderManagementContext : DbContext
{
    // TODO: DbSet<Customer> property
    // TODO: DbSet<Product> property
    // TODO: DbSet<Order> property
    // TODO: DbSet<OrderItem> property
    // TODO: DbSet<Payment> property
    // TODO: DbSet<Inventory> property
    // TODO: DbSet<Address> property
    // TODO: DbSet<Cart> property
    // TODO: DbSet<CartItem> property
    // TODO: DbSet<Discount> property
    
    // TODO: Constructor accepting DbContextOptions
    public OrderManagementContext(DbContextOptions<OrderManagementContext> options) 
        : base(options)
    {
    }
    
    // TODO: OnModelCreating for entity configuration and relationships
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // TODO: Configure Customer entity
        // TODO: Configure Product entity
        // TODO: Configure Order entity
        // TODO: Configure OrderItem entity
        // TODO: Configure Payment entity
        // TODO: Configure Inventory entity
        // TODO: Configure Address entity
        // TODO: Configure Cart entity
        // TODO: Configure CartItem entity
        // TODO: Configure Discount entity
        
        // TODO: Configure relationships (1-N, N-N)
        // TODO: Configure constraints (unique, required)
        // TODO: Configure indexes for performance
    }
}
