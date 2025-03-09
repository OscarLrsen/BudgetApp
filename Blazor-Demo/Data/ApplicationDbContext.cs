using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyBlazorApp.Models;

namespace MyBlazorApp.Data
{
    // Inherit from IdentityDbContext<User> so that the AspNetUsers table includes our Budget property.
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        { }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Expense> Expenses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision.
            modelBuilder.Entity<Expense>()
                        .Property(e => e.Amount)
                        .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<User>()
                        .Property(u => u.Budget)
                        .HasColumnType("decimal(18,2)");
        }
    }
}
