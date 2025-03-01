using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test02.Models;

namespace Test02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly AppDbcontext _context;

        public CategoryController(AppDbcontext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            //return await _context.Categories.Include(c => c.products).ToListAsync();
            return await _context.Categories.ToListAsync(); 
        }

        [HttpGet("getById/{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.Include(c => c.Id).FirstOrDefaultAsync(c => c.Id == id);
            if (category == null) return NotFound();
            return category;
        }

        [HttpPost("create")]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] createCateReq  req)
        {
            if (_context.Categories.Any(c => c.Name == req.Name) )
            {
                return BadRequest("Category Name was created");
            }
            var newCategory  = new Category { Id  = req.Id  , Name = req.Name};
          _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();  
            return Ok(newCategory);  
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest("ID in URL does not match the category ID.");
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                return NotFound("Category not found.");

            // Update only modified fields
            existingCategory.Name = category.Name;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Categories.Any(e => e.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null) return NotFound();
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        public class createCateReq
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
