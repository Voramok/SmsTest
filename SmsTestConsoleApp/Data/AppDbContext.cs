using Microsoft.EntityFrameworkCore;
using SmsTestConsoleApp.Models;

namespace SmsTestConsoleApp.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<MenuItem> Dishes { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MenuItem>().ToTable("MenuItems");
        }
    }
}
