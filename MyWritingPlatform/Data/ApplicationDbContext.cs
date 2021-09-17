using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWritingPlatform.Models;

namespace MyWritingPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        DbSet<Category> Categories { get; set; }
        DbSet<Post> Posts { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Сomment> Сomments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
