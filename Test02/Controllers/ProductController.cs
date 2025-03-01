using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Test02.Constants;
using Test02.Constants.metadata;
using Test02.Models;
using Test02.Payload.Response;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase

    {
        private readonly AppDbcontext _dbcontext;
        private readonly IConfiguration _configuration;
        public ProductController(AppDbcontext appDbcontext, IConfiguration configuration)
        {
            _dbcontext = appDbcontext;
            _configuration = configuration;
        }
        // GET: api/<ProductController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductRes>>> getAllProduct()
        {
            var productListResponse = await _dbcontext.Products.Select(p => new ProductRes
            {
                Id = p.Id,
                Name = p.Name,
                image = p.image,
                Quantity = p.Quantity,

            }).ToListAsync();
            if (productListResponse == null)
            {
                return NotFound();
            }


            return Ok(
                ApiResponseBuilder.BuildResponse(
                    message: "Get Product Success",
                    statusCode: StatusCodes.Status200OK,
                    data: productListResponse
                )
             );
        }

        // GET api/<ProductController>/5
        [HttpGet(ApiEndPointConstant.Product.ProductId)]
        public async Task<ActionResult<Product>> Get(int id)
        {
            var product = await _dbcontext.Products.Include(p => p.Id).FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound();
            ProductRes clone = new ProductRes();

            return product;
        }

        // POST api/<ProductController>
        [HttpPost("create")]
        public async Task<ActionResult<Product>> createProduct([FromBody] productReq req)
        {
            if (_dbcontext.Products.Any(p => p.Name == req.Name))
            {
                return BadRequest("name product created , Pls update quantity !");
            }
            var newProduct = new Product { Name = req.Name.Trim(), Price = req.Price, image = req.image.Trim() };
            _dbcontext.Products.Add(newProduct);
            await _dbcontext.SaveChangesAsync();
            return Ok(newProduct);
        }

        // PUT api/<ProductController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] JsonPatchDocument<Product> req)
        {


            var existingProduct = await _dbcontext.Products.FindAsync((id));
            if (existingProduct == null) return NotFound("Prodcut not found");

            //existingProduct.Name = req.Name;
            //existingProduct.Price = req.Price;
            //existingProduct.image = req.image;
            //existingProduct.CategoryId = req.CategoryId;

            req.ApplyTo(existingProduct, (Microsoft.AspNetCore.JsonPatch.Adapters.IObjectAdapter)ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();

        }

        // DELETE api/<ProductController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
    public class productReq
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public string image { get; set; }
        public int CategoryId { get; set; }
    }

}
