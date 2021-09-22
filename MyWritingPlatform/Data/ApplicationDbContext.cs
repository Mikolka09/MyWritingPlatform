using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyWritingPlatform.Models;

namespace MyWritingPlatform.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Comment> Сomments { get; set; }
        protected void OnModelCreatingPost(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Post>()
                    .HasMany(c => c.Tags)
                    .WithMany(s => s.Posts)
                    .UsingEntity(j => j.ToTable("PivotPostTag"));
        }


            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            OnModelCreatingPost(modelBuilder);
        }
    }
}
