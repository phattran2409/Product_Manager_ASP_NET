namespace Test02.Payload.Request
{
    public class AuthRequest
    {
         public  class Login
        {
            public  string UserName { get; set; }
            public  string Password { get; set; }
        }

        public  class Register
        {
            public  string Name { get; set; }
            public  string UserName { get; set; }
            public  string Email { get; set; }
            public  string Password { get; set; }
        }   
    }
}
