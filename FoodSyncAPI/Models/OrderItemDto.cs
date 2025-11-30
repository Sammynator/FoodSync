using FoodSyncAPI.Models.Entities;

namespace FoodSyncAPI.Models
{
    public class OrderItemDto
    {
        public MenuItem MenuItem { get; set; }
        public int Quantity { get; set; }
        public string Notes { get; set; }
    }
}
