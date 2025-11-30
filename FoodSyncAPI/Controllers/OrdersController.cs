using FoodSyncAPI.Data;
using FoodSyncAPI.Models;
using FoodSyncAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET all orders with items + menu items
        [HttpGet]
        public IActionResult GetAllOrders()
        {
            var allOrders = dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.MenuItem)
                .ToList();
            return Ok(allOrders);
        }

        // GET order by id with items + menu items
        [HttpGet("{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            return Ok(order);
        }

        // POST new order
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
                    MenuItemId = i.MenuItemId,
                    Quantity = i.Quantity,
                    Notes = i.Notes
                }).ToList()
            };

            dbContext.Orders.Add(order);
            dbContext.SaveChanges();

            // Reload order with items + menu items
            var orderWithItems = dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefault(o => o.Id == order.Id);

            return CreatedAtAction(nameof(GetOrder), new { id = orderWithItems.Id }, orderWithItems);
        }

        // PUT: update order or items
        [HttpPut("{id}")]
        public IActionResult AddOrderItem(int id, OrderDto dto)
        {
            var order = dbContext.Orders
                .Include(o => o.Items)
                .FirstOrDefault(o => o.Id == id);

            if (order == null)
                return NotFound();

            if (order.Status != "pending")
                return BadRequest("Only pending orders can be edited");

            // Update userId
            if (dto.UserId != 0)
                order.UserId = dto.UserId;

            // Update or add items
            foreach (var itemDto in dto.Items)
            {
                if (itemDto.Id != 0)
                {
                    var existingItem = order.Items.FirstOrDefault(i => i.Id == itemDto.Id);
                    if (existingItem != null)
                    {
                        if (itemDto.Quantity > 0)
                            existingItem.Quantity = itemDto.Quantity;

                        if (!string.IsNullOrEmpty(itemDto.Notes))
                            existingItem.Notes = itemDto.Notes;

                        if (itemDto.MenuItemId != 0)
                            existingItem.MenuItemId = itemDto.MenuItemId;
                    }
                    else
                    {
                        return BadRequest($"Item with Id {itemDto.Id} does not exist");
                    }
                }
                else
                {
                    // Add new item
                    order.Items.Add(new OrderItem
                    {
                        MenuItemId = itemDto.MenuItemId,
                        Quantity = itemDto.Quantity,
                        Notes = itemDto.Notes
                    });
                }
            }

            dbContext.SaveChanges();

            // Reload with menu items
            var updatedOrder = dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefault(o => o.Id == order.Id);

            return Ok(updatedOrder);
        }

        // PUT: update order status
        [HttpPut("{id}/status")]
        public IActionResult UpdateOrderStatus(int id, OrderWithStatusDto dto)
        {
            var order = dbContext.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.MenuItem)
                .FirstOrDefault(o => o.Id == id);

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
                return BadRequest($"Cannot change status from '{oldStatus}' to '{newStatus}'");
            }

            order.Status = newStatus;
            dbContext.SaveChanges();

            return Ok(order);
        }
    }
}
