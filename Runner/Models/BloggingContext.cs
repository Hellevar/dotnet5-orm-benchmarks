using Microsoft.EntityFrameworkCore;

namespace Runner.Models
{
    public class BloggingContext : DbContext
    {
        private readonly string _connectionString;

        public BloggingContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString).UseSnakeCaseNamingConvention();
        }
    }
}
