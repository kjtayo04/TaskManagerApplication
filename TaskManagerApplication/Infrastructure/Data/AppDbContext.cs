using Microsoft.EntityFrameworkCore;
using TaskManagerApplication.Domain.Entities;

namespace TaskManagerApplication.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> Tasks { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TaskItem>(b =>
            {
                b.HasKey(t => t.Id);
                b.Property(t => t.Title).IsRequired();
                b.Property(t => t.Priority).HasDefaultValue("Low");
                b.Property(t => t.Status).HasDefaultValue("todo");
            });
        }
    }
}
