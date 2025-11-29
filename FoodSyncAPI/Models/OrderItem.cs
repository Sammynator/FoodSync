namespace FoodSyncAPI.Models
{
    public class OrderItem
    {
            public int Id { get; set; }
            public string MenuItem { get; set; }
            public int Quantity { get; set; }
            public string Notes { get; set; }
        }
}
