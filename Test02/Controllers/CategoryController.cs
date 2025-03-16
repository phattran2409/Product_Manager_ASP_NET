using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using Test02.Constants;
using Test02.Constants.metadata;
using Test02.Models;
using Test02.Payload.Response;

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

        [HttpGet(ApiEndPointConstant.Category.Categories)]
        public async Task<ActionResult<CategoryDTO>> GetCategories()
        {
            try
            {
                var category = await _context.Categories.Select(c => new CategoryDTO
                {
                    Id = c.Id,
                    Name = c.Name
                }).ToListAsync();

                if (category == null)

                {
                    return NotFound(
                         ApiResponseBuilder.BuildErrorResponse<IEnumerable<CategoryDTO>>(
                            data: null,
                            message: "Category not found",
                            statusCode: StatusCodes.Status404NotFound,
                            reason: "Resource not found"
                        )
                    );
                }

                var response = ApiResponseBuilder.BuildResponse<IEnumerable<CategoryDTO>>(
                    data: category,
                    message: "Get Categories Success",
                    statusCode: StatusCodes.Status200OK
                );  

                return Ok(response);  
            } catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }   

        }

        [HttpGet(ApiEndPointConstant.Category.CategoryId)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CategoryDTO>> GetCategory(int id)
        {
            var res = await _context.Categories.Include(c => c.Id).FirstOrDefaultAsync(c => c.Id == id);
            if (res == null) return NotFound();

            if (res == null)
            {

                var notFound = ApiResponseBuilder.BuildErrorResponse<CategoryDTO>(
                    data: null,
                    message: $"Category with ID {id} not found",
                    statusCode: StatusCodes.Status404NotFound,
                    reason: "Resource not found"
                );

                return NotFound(notFound);
            }
            var category = new CategoryDTO()
            {
                Id = res.Id,
                Name = res.Name
            };
            var categoryRespone = ApiResponseBuilder.BuildResponse<CategoryDTO>(
                data: category,
                message: "Get Category Success",
                statusCode: StatusCodes.Status200OK

            );
            return Ok(categoryRespone);


        }

        [HttpPost(ApiEndPointConstant.Category.CreateCategory)]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] createCateReq req)
        {
            if (_context.Categories.Any(c => c.Name == req.Name))
            {
                return BadRequest("Category Name was created");
            }
            var newCategory = new Category { Id = req.Id, Name = req.Name };
            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();


            return Ok(ApiResponseBuilder.BuildResponse<Category>(
                 statusCode: StatusCodes.Status200OK,
                 message: "Create Category Success",
                 data: newCategory
             ));
        }

        [HttpPut(ApiEndPointConstant.Category.UpdateCategory)]
        public async Task<IActionResult> UpdateCategory(int id, Category category)
        {
            if (id != category.Id)
                return BadRequest("ID in URL does not match the category ID.");
            var existingCategory = await _context.Categories.FindAsync(id);
            if (existingCategory == null)
                return NotFound("Category not found.");

            // Update only modified fields
            existingCategory.Name = category.Name.Length > 0 ?  category.Name : existingCategory.Name;
            return Ok(new
            {
                message = "Update Category Success",
                statusCode = StatusCodes.Status200OK,
            }
             );
        }

        [HttpDelete(ApiEndPointConstant.Category.DeleteCategory)]
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
