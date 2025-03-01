using System.Security.Cryptography.X509Certificates;

namespace Test02.Constants
{
    public class ApiEndPointConstant
    {
        public const string RootEndPoint = "/Api";
        public const string ApiVersion = "/v1"; 
        public const string ApiEndPoint = RootEndPoint + ApiVersion;    
        
        public static class Auth
        {
            
        }

        public static class Product
        {
            public const string Products = ApiEndPoint + "/products";
            public const string ProductId = Products + "/{id}";
            public const string UpdateProduct = ProductId + "/update";
            public const string DeleteProduct = Products + "/Delete/{id}";   
         
        }
        
    }
}
