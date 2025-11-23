using Assignment.Data;
using Assignment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin,Staff")]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public CategoriesController(AppDbContext db) { _db = db; }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var list = await _db.Categories.ToListAsync();
            return Ok(list);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Category model)
        {
            model.Id = Guid.NewGuid();
            _db.Categories.Add(model);
            await _db.SaveChangesAsync();
            return Ok(model);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] Category model)
        {
            var cate = await _db.Categories.FindAsync(id);
            if (cate == null) return NotFound();

            cate.Name = model.Name;
            cate.IsActive = model.IsActive;
            await _db.SaveChangesAsync();
            return Ok(cate);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var cate = await _db.Categories.FindAsync(id);
            if (cate == null) return NotFound();

            _db.Categories.Remove(cate);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
