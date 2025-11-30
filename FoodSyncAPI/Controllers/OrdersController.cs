using FoodSyncAPI.Data;
using FoodSyncAPI.Models;
using FoodSyncAPI.Models.Entities;
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
        public IActionResult AddOrder(OrderDto dto)
        {
            if (dto == null)
                return BadRequest();

var order = new Order
    {
        UserId = dto.UserId,
        Status = "pending",
        Items = dto.Items.Select(i => new OrderItem
        {
            MenuItem = i.MenuItem,
            Quantity = i.Quantity,
            Notes = i.Notes
        }).ToList()
    };

            dbContext.Orders.Add(order);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPut("{id}")]

        public IActionResult AddOrderItem(int id, OrderDto dto)
        {
            var order = dbContext.Orders.Find(id);

            if (order == null)
                return NotFound();

            if (order.Status != "pending")
                return BadRequest("Only pending orders can be edited");

// 1️⃣ Update UserId if provided
    if (dto.UserId != 0) 
        order.UserId = dto.UserId;

    // 2️⃣ Update existing items
    foreach (var itemDto in dto.Items)
    {
        if (itemDto.Id != 0) // existing item
        {
            var existingItem = order.Items.FirstOrDefault(i => i.Id == itemDto.Id);
            if (existingItem != null)
            {
                    existingItem.MenuItem = itemDto.MenuItem;

                if (itemDto.Quantity > 0)
                    existingItem.Quantity = itemDto.Quantity;

                if (!string.IsNullOrEmpty(itemDto.Notes))
                    existingItem.Notes = itemDto.Notes;
            }
            else
            {
                return BadRequest($"Item with Id {itemDto.Id} does not exist");
            }
        }
        else
        {
            // Optional: add new item if you want
            order.Items.Add(new OrderItem
            {
                MenuItem = itemDto.MenuItem,
                Quantity = itemDto.Quantity,
                Notes = itemDto.Notes
            });
        }
    }


            dbContext.SaveChanges();
            return Ok(order);
        }

        [HttpPut("{id}/status")]

        public IActionResult UpdateOrderStatus( int id, OrderWithStatusDto dto)
        {
            var order = dbContext.Orders.Find(id);

            if (order == null)
                return NotFound();

            string oldStatus = order.Status;
            string newStatus = dto.Status;

            var validTransitions = new Dictionary<string, List<string>>
            {
        { "pending", new List<string> { "preparing", "done" } },
        { "preparing", new List<string> { "done" } },
        { "done", new List<string>() },
        { "served", new List<string>() } 
            };

            if (!validTransitions.ContainsKey(oldStatus) ||
                !validTransitions[oldStatus].Contains(newStatus))
            {
                return BadRequest($"Cannot change status from '{order.Status}' to '{dto.Status}'");
            }

            order.Status = dto.Status;
            dbContext.SaveChanges();

            return Ok(order);
        }

    }

}
