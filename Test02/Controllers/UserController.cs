
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Test02.Constants;
using Test02.Constants.metadata;
using Test02.Models;
using Test02.Payload.Request;
using Test02.Payload.Response;



namespace Test02.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbcontext _dbcontext;   

        public UserController(AppDbcontext appDbcontext)
        {
            _dbcontext = appDbcontext;  
        }   

        [HttpGet(ApiEndPointConstant.user.Users)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetUsers([FromQuery] PaginationParams paginationParams ,[FromQuery] UserReq req)
        {
            try
            {
                var totalItems = await _dbcontext.Users.CountAsync();

                if (totalItems == 0)
                {
                    return NotFound(new
                    {
                        message = "User not found",
                    });
                }

                IQueryable<User> query = _dbcontext.Users;

                if (!string.IsNullOrEmpty(req.search))
                {
                    query = query.Where(p => p.Name.Contains(req.search) || p.UserName.Contains(req.search));
                }

                if (!string.IsNullOrEmpty(req.Email))
                {
                    query = query.Where(x => x.Email == req.Email);
                }
                if (!string.IsNullOrEmpty(req.status))
                {
                    query = query.Where(x => x.status == req.status);
                }   

                if (!string.IsNullOrEmpty(req.role))
                {
                    query = query.Where(x => x.role == req.role);
                }

                var users = await query.Select(p => new UserDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    UserName = p.UserName,
                    Email = p.Email,
                    status = p.status,
                    role = p.role
                }).Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize).Take(paginationParams.PageSize).ToListAsync();


                var result = new PageResult<UserDTO>
                {
                    Data = users,
                    CurrentPage = paginationParams.PageNumber,
                    PageSize = paginationParams.PageSize,
                    TotalItems = totalItems,
                };

                var successResponse = ApiResponseBuilder.BuildPageResponse<UserDTO>(
                    items: users,
                    totalPages: result.TotalPages,
                    totalItems: totalItems,
                    message: "Get User Success",
                    currentPage: result.CurrentPage,
                    pageSize: result.PageSize,
                    hasNext: result.HasNext,
                    hasPrevious: result.HasPrevious
                );




                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            } 
        }

        [HttpPost(ApiEndPointConstant.user.CreateUser)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> CreateUser([FromBody] UserDTO newUser)
        {
            try
            {
                IQueryable<User> query = _dbcontext.Users;
                if (newUser == null)
                {
                    return BadRequest(new
                    {
                        message = "User is required",
                    });
                }   
                
                if (!string.IsNullOrEmpty(newUser.UserName) || !string.IsNullOrEmpty(newUser.Email))
                {
                     var existUserName = query.Where(x => x.UserName == newUser.UserName).FirstOrDefault(); 
                     var existEmail = query.Where(x => x.Email == newUser.Email).FirstOrDefault();  
                    if (existUserName != null)
                    {
                        return BadRequest(new
                        {
                            message = "Username already exist",
                            StatusCode = StatusCodes.Status400BadRequest    
                        });
                    }

                    if (existEmail != null)
                    {
                        return BadRequest(new
                        {
                            message = "Email already exist",
                            StatusCode = StatusCodes.Status400BadRequest
                        }); 
                    }
                }   

                var newUserModel = new User
                {
                    Name = newUser.Name,
                    UserName = newUser.UserName,
                    Email = newUser.Email,
                    status = newUser.status,
                    role = newUser.role
                };
                await _dbcontext.Users.AddAsync(newUserModel); 
                
                await _dbcontext.SaveChangesAsync();  
                
                var successReponse = ApiResponseBuilder.BuildResponse<User>(
                    data: newUserModel,
                    statusCode: StatusCodes.Status201Created,
                    message: "Create User Success"  
                );  

                return Ok(successReponse);    

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Unauthorized"});
            }
        }

        [HttpPut(ApiEndPointConstant.user.UpdateUser)]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserDTO>>> UpdateUser([FromBody] UserDTO updateUser)
        {
            try
            {
                IQueryable<User> query = _dbcontext.Users;
                var UserIdFromToken = GetUserIdFromToken();  
                var existUser = query.Where(x => x.Id == updateUser.Id).FirstOrDefault();
                
                if (existUser == null)
                {
                    return NotFound(new
                    {
                        message = "User not found",
                        StatusCode = StatusCodes.Status404NotFound
                    }); 
                }

                var isAdmin = User.IsInRole("Admin");
                if (!isAdmin && existUser.Id.ToString() != UserIdFromToken)
                {
                    return Forbid();
                }

                
                if (!string.IsNullOrEmpty(updateUser.UserName))
                {
                    existUser.UserName = updateUser.UserName;  
                }
                if (!string.IsNullOrEmpty(updateUser.Email))
                {
                    existUser.Email = updateUser.Email;
                }

                if (!string.IsNullOrEmpty(updateUser.Name))
                {
                    existUser.Name = updateUser.Name;
                } 
                
                return Ok();
            }
            catch (Exception e)
            {
               return StatusCode(500, new { message = "Unauthorized" });     
            }  
        }

        public string GetUserIdFromToken()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return idClaim;
        }   
    }
}
