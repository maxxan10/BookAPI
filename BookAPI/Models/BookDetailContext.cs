using Microsoft.EntityFrameworkCore;

namespace BookAPI.Models
{
    public class BookDetailContext : DbContext
    {
        public BookDetailContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BookDetail> BookDetails { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) //To server
        {
            modelBuilder.Entity<User>().ToTable("users");
        }
    }
}
