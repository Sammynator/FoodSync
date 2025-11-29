using FoodSyncAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace FoodSyncAPI.Data;
public class FoodSyncContext : DbContext
{
    public FoodSyncContext(DbContextOptions<FoodSyncContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
}

