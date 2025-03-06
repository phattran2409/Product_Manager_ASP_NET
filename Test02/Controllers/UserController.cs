using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
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
         private readonly AppDbcontext  _context;
        private  readonly IConfiguration _configuration;
        
        public UserController(AppDbcontext appDbcontext, IConfiguration configuration)
        {
            _context  = appDbcontext;  
            _configuration = configuration; 
        }

        [HttpPost(ApiEndPointConstant.Auth.authRegister)]
        public async Task<IActionResult> SignUp([FromBody] AuthRequest.Register request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
            {
                return BadRequest("Email already exists");
            }
             
            var passwordHash  = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User { UserName = request.UserName, Password = passwordHash, Name = request.Name, Email = request.Email, role = "user" , status = "active" };  

            _context.Users.Add(user);   
            
            await _context.SaveChangesAsync();
            return Ok(
                new {
                    message = "Register success",
                    statusCode =  StatusCodes.Status200OK,
                }
            );
            
          
        }

        [HttpPost(ApiEndPointConstant.Auth.authLogin)]
        public ActionResult<IEnumerable<UserDTO>> Login([FromBody] AuthRequest.Login req)
        {
            var userLogin = _context.Users.FirstOrDefault(u => u.UserName == req.UserName);
            if (userLogin == null || !BCrypt.Net.BCrypt.Verify(req.Password, userLogin.Password))
            {
                return Unauthorized("Invalid Username or password");
            }
            var token = GenerateJwtToken(userLogin);
            UserDTO userResponse = new UserDTO
            {
                Id = userLogin.Id,
                Name = userLogin.Name,
                UserName = userLogin.UserName,
                Email = userLogin.Email,
                status = userLogin.status,
                role = userLogin.role
            };
            return Ok(new { Token = token, user =  userResponse});
        }

        private string GenerateJwtToken(User user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key , SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName), 
                new Claim(ClaimTypes.Role , user.role), 
            };
       
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],       
                _configuration["Jwt:Audience"], 
                claims : claims,
                expires : DateTime.Now.AddHours(1),
                signingCredentials  : creds
            ); 
            
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Admin")]
         public IActionResult getProfileDashboard()
        {
            return Ok(new { Message = "THIS IS A FUNCTION OF ADMIN ! " });
        }
    }

}
