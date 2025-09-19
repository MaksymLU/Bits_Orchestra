using Contact_Manager_Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Contact_Manager_Application.Db
{
    public class AdmissionsDbContext : DbContext
    {
        public AdmissionsDbContext(DbContextOptions<AdmissionsDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.UserId);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Name)
                .IsUnique(false);
        }
    }
}
