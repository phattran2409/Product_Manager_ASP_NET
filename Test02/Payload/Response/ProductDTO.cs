namespace Test02.Payload.Response
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public decimal? Price { get; set; }
        public decimal? Quantity { get; set; }

        public string? image { get; set; }

        public int? CategoryId { get; set; }
    }
}
