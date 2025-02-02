using Microsoft.EntityFrameworkCore;
using Enterprise.EmployeeManagement.DAL;
using System.ComponentModel.DataAnnotations;

namespace Enterprise.EmployeeManagement.DAL.Models
{
    
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SiteUserModel> SiteUsers { get; set; }
        public DbSet<RoleModel> Roles { get; set; }

        public DbSet<TaskModel> Tasks { get; set; } 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Optional: Add any custom configurations for the model here
            modelBuilder.Entity<SiteUserModel>()
                .Property(u => u.CreateDate)
                .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Sets default value for CreateDate to UTC now (if using SQL Server)

            //modelBuilder.Entity<TaskModel>()
            //    .Property(u => u.AssignedDate)
            //    .HasDefaultValueSql("CURRENT_TIMESTAMP"); // Sets default value for AssignedDate to UTC now (if using SQL Server)

            //modelBuilder.Entity<SiteUserModel>(entity => {
            //    entity.HasIndex(e => e.EmailAddress).IsUnique();
            //});
        }
    }
}
