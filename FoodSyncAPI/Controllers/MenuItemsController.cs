using FoodSyncAPI.Data;
using FoodSyncAPI.Models;
using FoodSyncAPI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
    }
}
