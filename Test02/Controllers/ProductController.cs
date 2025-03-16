using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic;
using System.Net.WebSockets;
using Test02.Constants;
using Test02.Constants.metadata;
using Test02.Models;
using Test02.Payload.Request;
using Test02.Payload.Response;
using Test02.Repository.Product;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Test02.Constants.ApiEndPointConstant;
using Product = Test02.Models.Product;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Test02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase

    {
        private readonly AppDbcontext _dbcontext;
        private readonly IConfiguration _configuration;
        //private readonly ItemRepository _repository;
        public ProductController(AppDbcontext appDbcontext, IConfiguration configuration )
        {
            _dbcontext = appDbcontext;
            _configuration = configuration;
            //_repository = repository;    
        }
        // GET: api/<ProductController>
        [HttpGet(ApiEndPointConstant.Product.Products)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> getAllProduct([FromQuery] PaginationParams  paginationParams , ProductReq req)
        {
            var totalItems = await _dbcontext.Products.CountAsync();
            
            IQueryable<Product> query = _dbcontext.Products;  
            if (totalItems == 0)
            {
                return NotFound(new
                {
                    message = "No product found",
                    statusCode = StatusCodes.Status404NotFound
                });
            }       
            if (!string.IsNullOrEmpty(req.search))
            {
                query = query.Where(p => p.Name.Contains(req.search.Trim()));
            }

            if (req.isNew.HasValue || req.isSale.HasValue)
            {
                if (req.isNew == true || req.isSale == true)
                {
                    if (req.isNew == true)
                    {
                        query = query.Where(p => p.isNew == true);
                    }

                    if (req.isSale == true)
                    {
                        query = query.Where(p => p.isSale == true);
                    }
                }
                else
                {
                    if (req.isNew == false)
                    {
                        query = query.Where(p => p.isNew == false);
                    }

                    if (req.isSale == false)
                    {
                        query = query.Where(p => p.isSale == false);
                    }
                }    
            }

            if (req.categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == req.categoryId);  
            }
            
            if (!string.IsNullOrEmpty(req.sort))
            {
                var sortParts = req.sort.Split('-');   
                var sortField = sortParts[0];   
                var sortDirection = sortParts.Length > 1 ? sortParts[1]?.Trim().ToLower() : "asc";
                switch (sortField)
                {
                    case "name":
                        query = sortDirection == "desc" ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name);
                        break;
                    case "price":
                        query  = sortDirection == "desc" ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price);
                        break;
                    case "quantity":
                        query = sortDirection == "desc" ? query.OrderByDescending(p => p.Quantity) : query.OrderBy(p => p.Quantity);
                        break;     
                    default: 
                        query = query.OrderBy(p => p.Id);
                        break;
                }
            }else
            {
                query = query.OrderBy(p => p.Id);    
            }

            var productListResponse = await query.Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                image = p.image,
                Quantity = p.Quantity,
                Price = p.Price,
                CategoryId = p.CategoryId,
                isNew = p.isNew,
                isSale = p.isSale 
            }).Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToListAsync();
            

            var result = new PageResult<ProductDTO>
            {
                Data = productListResponse,
                CurrentPage = paginationParams.PageNumber,
                PageSize = paginationParams.PageSize,
                TotalItems = totalItems,
            };
             

            if (productListResponse == null)
            {   
                return NotFound(new
                {
                    message = "No product found",
                    statusCode = StatusCodes.Status404NotFound
                });
            }
            var successReponse = ApiResponseBuilder.BuildPageResponse<ProductDTO>(
                  items: productListResponse,
                  totalPages: result.TotalPages,
                  totalItems: totalItems,
                  message: "Get Product Success",
                  currentPage: result.CurrentPage,
                  pageSize: result.PageSize , 
                  hasNext : result.HasNext, 
                  hasPrevious: result.HasPrevious
             );

            return Ok(
                successReponse
             );
        }

        // GET api/<ProductController>/5
        [HttpGet(ApiEndPointConstant.Product.ProductId)]
        public async Task<ActionResult<ProductDTO>> Get(int id)
        {
            try
            {
                var product = await _dbcontext.Products.FindAsync(id);  
                if (product == null)
                {
                    return NotFound(new
                    {
                        statusCode = StatusCodes.Status404NotFound,  
                        message  = "Not Found",
                    });
                }
                var successResponse = ApiResponseBuilder.BuildResponse<ProductDTO>(
                    message: "Get Product Success",
                    statusCode: StatusCodes.Status200OK,
                    data: new ProductDTO
                    {
                        Id = product.Id,
                        Name = product.Name,
                        image = product.image,
                        Quantity = product.Quantity,
                        Price = product.Price,
                        CategoryId = product.CategoryId
                    }
                );
                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Get failed", error = ex.Message });
            }   
            
        }

        // POST api/<ProductController>
        [HttpPost(ApiEndPointConstant.Product.CreateProduct)]
        public async Task<ActionResult<ProductDTO>> createProduct([FromBody] ProductDTO req)
        {
            if (_dbcontext.Products.Any(p => p.Name == req.Name))
            {
                return BadRequest("name product created , Pls update quantity !");
            }
            if(req ==  null || string.IsNullOrWhiteSpace(req.Name) || req.Quantity < 0)
            {
                return BadRequest(new
                {
                    message = "Invalid product data. Name and non-negative Quantity are required.",
                    data = req
                });
            }
            
            var newProduct = new Product { Name = req.Name.Trim(), Price = req.Price, image = req.image.Trim() , Quantity = req.Quantity , CategoryId = req.CategoryId };
            
            _dbcontext.Products.Add(newProduct);
            await _dbcontext.SaveChangesAsync();

            var responseSuccess = ApiResponseBuilder.BuildResponse(
                message: "Create Product Success",
                statusCode: StatusCodes.Status201Created,
                data: newProduct
            );
            return Ok(responseSuccess);
        }

        // PUT api/<ProductController>/5
        [HttpPut(ApiEndPointConstant.Product.UpdateProduct)]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] ProductDTO req)
        {


            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "Validation failed", errors = ModelState });
            }

            try
            {
                var existingProduct = await _dbcontext.Products.FindAsync((id));
                if (existingProduct == null)
                {
                    var responseFail = ApiResponseBuilder.BuildErrorResponse<Product>(
                        data: null,
                        message: $"Product with ID {id} not found",
                        statusCode: StatusCodes.Status404NotFound,
                        reason: "Resource not found"
                    );
                    return NotFound(responseFail);
                }



                existingProduct.Name = req.Name ?? existingProduct.Name;
                existingProduct.Price = req.Price.HasValue ? req.Price : existingProduct.Price;
                existingProduct.image = req.image ?? existingProduct.image;
                existingProduct.CategoryId = req.CategoryId.HasValue ? req.CategoryId : existingProduct.CategoryId;
                existingProduct.Quantity = req.Quantity.HasValue ? req.Quantity : existingProduct.Quantity;

                await _dbcontext.SaveChangesAsync();

                var upateSuccess = ApiResponseBuilder.BuildResponse<ProductDTO>(
                   message: $"Product with ID {id} updated successfully",
                    data: new ProductDTO
                    {
                        Name = existingProduct.Name,
                        Price = existingProduct.Price,
                        image = existingProduct.image,
                        CategoryId = existingProduct.CategoryId
                    },
                    statusCode: StatusCodes.Status200OK
                );
                return Ok(upateSuccess);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Update failed", error = ex.Message });
            }
        }

        // DELETE api/<ProductController>/5
        [HttpDelete(ApiEndPointConstant.Product.DeleteProduct)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var existingProduct = await _dbcontext.Products.FindAsync(id);
                if (existingProduct == null)
                {
                    return NotFound(new
                    {
                        message = $"Product with ID {id} not found",
                        statusCode = StatusCodes.Status200OK
                    }
                    );
                }
                _dbcontext.Products.Remove(existingProduct);
                await _dbcontext.SaveChangesAsync();
                var successResponse = ApiResponseBuilder.BuildResponse<ProductDTO>(
                statusCode: StatusCodes.Status200OK,
                 message: $"Product with ID {id} has been permanently deleted",
                 data: null
        );
                return Ok(successResponse);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Delete failed", error = ex.Message });
            }

        }
    }
    //public class productReq
    //{
    //    public string Name { get; set; }
    //    public decimal Price { get; set; }
    //    public decimal Quantity { get; set; }
    //    public string image { get; set; }
    //    public int CategoryId { get; set; }
    //}

}
