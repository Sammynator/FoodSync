using FoodSyncAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoodSyncAPI.Data;
public class FoodSyncContext : DbContext
{
    public FoodSyncContext(DbContextOptions<FoodSyncContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasPrecision(18, 2); // avoids truncation
    }
}

