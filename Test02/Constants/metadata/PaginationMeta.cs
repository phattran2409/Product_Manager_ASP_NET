using System.Text.Json.Serialization;

namespace Test02.Constants.metadata
{
    public class PaginationMeta
    {
        [JsonPropertyName("totalPages")] 
        public int TotalPages { get; set; }

        [JsonPropertyName("totalItems")]    
        public long TotalItems { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("currentPage")]
        public int CurrentPage { get; set; }
         

        [JsonPropertyName("hasNext")]
        public bool hasNext { get; set; }

        [JsonPropertyName("hasPrevious")] 
        public bool hasPrevious { get; set; }   

    }
}
