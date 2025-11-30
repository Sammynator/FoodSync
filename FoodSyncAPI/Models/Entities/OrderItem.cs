namespace FoodSyncAPI.Models.Entities
{
    public class OrderItem
    {
            public int Id { get; set; }
            public int MenuItemId { get; set; }
            public MenuItem MenuItem { get; set; }
            public int Quantity { get; set; }
            public string Notes { get; set; }
        }
}
