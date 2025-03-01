using Microsoft.EntityFrameworkCore;

namespace Test02.Models
{
    public class AppDbcontext  : DbContext 
    {
    

        public AppDbcontext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<blog> Blogs { get; set; }
        public DbSet<post> Posts { get; set; }
        public DbSet<Category> Categories { get; set; }
      
        //  relationship user to product 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ////base.OnModelCreating(modelBuilder);
            //modelBuilder.Entity<User>().Property(u => u.Id).ValueGeneratedOnAdd();

            //modelBuilder.Entity<blog>().HasMany(e => e.Posts).WithOne(p => p.blog).HasForeignKey(e => e.Blogid).IsRequired(false);

          


        }
    }
}
