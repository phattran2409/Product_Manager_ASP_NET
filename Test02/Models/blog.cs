namespace Test02.Models
{
    public class blog
    {
        public int Id { get; set; } 
        public string Title { get; set; }   
        public string Description { get; set; } 
        public string Author { get; set; }
        
        public ICollection<post> Posts { get;} = new List<post>();  
        
    }
}
