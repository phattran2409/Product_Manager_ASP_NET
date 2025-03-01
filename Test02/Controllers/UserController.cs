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
using Test02.Models;


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

        [HttpPost("SignUp")]
        public async Task<IActionResult> SignUp([FromBody] SignUpRequest request)
        {
            if (_context.Users.Any(u => u.UserName == request.UserName))
            {
                return BadRequest("UserName already exists");
            }
             
            var passwordHash  = BCrypt.Net.BCrypt.HashPassword(request.Password);
            var user = new User { UserName = request.UserName, Password = passwordHash, Name = request.name, Email = request.email , role = "user" , status = "active" };  

            _context.Users.Add(user);   
            
            await _context.SaveChangesAsync();  
            return Ok("User Registered susccesFully");    
          
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var userLogin = _context.Users.FirstOrDefault(u => u.UserName == req.UserName);
            if (userLogin == null ||  !BCrypt.Net.BCrypt.Verify(req.Password , userLogin.Password))
            {
               return Unauthorized("Invalid Username or password");
            }
            var token = GenerateJwtToken(userLogin);
            return Ok( new {Token = token});
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

    public class SignUpRequest
    {
         public string UserName { get; set; }   
        public string Password { get; set; }    
        public string name { get; set; } 
        public string email { get; set; }    

            
    }

    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
