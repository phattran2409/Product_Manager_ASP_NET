namespace Test02.Payload.Request
{
    public class ProductReq
    {
        public  string? name { get; set; }  
        public decimal? price { get; set; }
        public string? description { get; set; }
        public string? image { get; set; }   
        public string? search { get; set; }
        public string? sort { get; set; }
        public bool? isNew { get; set; }
        public bool? isSale { get; set; }
        public int? categoryId { get; set; }    

    }
}
