using FoodSyncAPI.Models.Entities;

namespace FoodSyncAPI.Models
{
    public class OrderDto
    {

        public int UserId { get; set; } // who made the order
        public List<OrderItem> Items { get; set; } = new();
    }
}
