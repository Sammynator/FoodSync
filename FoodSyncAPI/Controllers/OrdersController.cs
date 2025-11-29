using FoodSyncAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace FoodSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly FoodSyncContext dbContext;

        public OrdersController(FoodSyncContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var allorders = dbContext.Orders.ToList();
            return Ok(allorders);
        }

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = dbContext.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            return Ok(order);
        }

        [HttpPost]

        public IActionResult AddOrder([FromBody] Models.Order newOrder)
        {
            dbContext.Orders.Add(newOrder);
            dbContext.SaveChanges();
            return CreatedAtAction(nameof(GetOrder), new { id = newOrder.Id }, newOrder);
        }
    }

}
