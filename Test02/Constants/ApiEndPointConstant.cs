using System.Security.Cryptography.X509Certificates;

namespace Test02.Constants
{
    public class ApiEndPointConstant
    {
        public const string RootEndPoint = "/api";
        public const string ApiVersion = "/v1"; 
        public const string ApiEndPoint = RootEndPoint + ApiVersion;    
        
        public static class Auth
        {
            public const string authLogin = ApiEndPoint + "/auth/login";   
            public const string authRegister = ApiEndPoint + "/auth/register";  

        }

        public static class Product
        {
            public const string Products = ApiEndPoint + "/products";
            public const string ProductId = Products + "/{id}";
            public const string CreateProduct = Products + "/create";
            public const string UpdateProduct = Products + "/update/{id}";
            public const string DeleteProduct = Products + "/delete/{id}";   
         
        }
        public static class Category
        {
            public const string Categories = ApiEndPoint + "/categories";
            public const string CategoryId = Categories + "/{id}";
            public const string CreateCategory = Categories + "/create";    
            public const string UpdateCategory = Categories + "/update/{id}";
            public const string DeleteCategory = Categories + "/delete/{id}";
        }   

    }
}
