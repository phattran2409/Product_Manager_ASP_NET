using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Test02.Models
{
    public class User 
    {
        public int Id { get; set; } 
        public string Name { get; set; }    
        public  string  UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage ="Invalid Email Format")]
        public string Email { get; set; }

        [Required (ErrorMessage = "password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 character")]
        public string Password { get; set; }


        public string status { get; set; }  

        public string role { get; set; } = "user";

        

    }
}
