using FoodSyncAPI.Data;
using FoodSyncAPI.Models;
using FoodSyncAPI.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FoodSyncAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MenuItemsController : ControllerBase
    {
        private readonly FoodSyncContext dbContext;

        public MenuItemsController(FoodSyncContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpGet]
        public IActionResult GetAllMenuItems()
        {
            var allMenuItems = dbContext.MenuItems.ToList();
            return Ok(allMenuItems);
        }

        [HttpGet("{id}")]
        public IActionResult GetMenuItem(int id)
        {
            var menuItem = dbContext.MenuItems.Find(id);
            if (menuItem == null)
            {
                return NotFound();
            }
            return Ok(menuItem);
        }

        [HttpPost]
        public IActionResult AddMenuItem(MenuItemDto dto)
        {
            if (dto == null)
                return BadRequest();

            var menuItem = new MenuItem
            {
                Name = dto.Name,
                Price = dto.Price
            };

            dbContext.MenuItems.Add(menuItem);
            dbContext.SaveChanges();

            return CreatedAtAction(nameof(GetMenuItem), new { id = menuItem.Id }, menuItem);
        }

        // PUT: update a menu item
        [HttpPut("{id}")]
        public IActionResult UpdateMenuItem(int id, MenuItemDto dto)
        {
            var menuItem = dbContext.MenuItems.Find(id);
            if (menuItem == null)
                return NotFound();

            if (!string.IsNullOrEmpty(dto.Name))
                menuItem.Name = dto.Name;

            if (dto.Price > 0)
                menuItem.Price = dto.Price;

            dbContext.SaveChanges();

            return Ok(menuItem);
        }

        // DELETE: remove a menu item
        [HttpDelete("{id}")]
        public IActionResult DeleteMenuItem(int id)
        {
            var menuItem = dbContext.MenuItems.Find(id);

            if (menuItem == null)
                return NotFound();

            dbContext.MenuItems.Remove(menuItem);
            dbContext.SaveChanges();

            return NoContent();
        }
    }
}
