using System.Text.Json.Serialization;

namespace Test02.Constants.metadata
{
    public class PagingReponse<T>
    {
        [JsonPropertyName("items")]
        public IEnumerable<T> Items { get; set; }

        [JsonPropertyName("meta")]    
        public  PaginationMeta meta { get; set; }
    }
}
