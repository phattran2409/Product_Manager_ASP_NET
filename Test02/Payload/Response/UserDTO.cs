using System.ComponentModel.DataAnnotations;

namespace Test02.Payload.Response
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserName { get; set; }    

        public string Email { get; set; }

        public string status { get; set; }

        public string role { get; set; } = "user";
    }
}
