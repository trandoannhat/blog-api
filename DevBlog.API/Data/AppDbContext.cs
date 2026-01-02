using DevBlog.API.Entities;
using Microsoft.EntityFrameworkCore;

namespace DevBlog.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<Post> Posts => Set<Post>();
    }
}