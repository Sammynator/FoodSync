namespace FoodSyncAPI.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; } // who made the order
        public string Status { get; set; } = "pending"; // pending, preparing, done
        public List<OrderItem> Items { get; set; } = new();
    }
}