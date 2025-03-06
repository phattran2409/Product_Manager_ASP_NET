using Microsoft.AspNetCore.Components.Forms;
using System.Text.Json.Serialization;

namespace Test02.Constants.metadata
{
    public class ApiResponse<T>
    {

        [JsonPropertyName("status_code")]
        public int StatusCode { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("reason")]
        public string? Reason { get; set; }

        [JsonPropertyName("is_success")]
        public bool IsSuccess { get; set; }

        [JsonPropertyName("data")]
        
        public T? Data { get; set; }

        //[JsonPropertyName("pageNumber")]
        //public int PageNumber { get; set; }

        //[JsonPropertyName("pageSize")]

        //public int PageSize { get; set; }

        //[JsonPropertyName("totalItems")]
        //public int TotalItems { get; set; }

        //[JsonPropertyName("totalPages")] 
        //public int TotalPages { get; set; }



    }
}
