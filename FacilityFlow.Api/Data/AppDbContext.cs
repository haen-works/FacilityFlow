using FacilityFlow.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace FacilityFlow.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<FaultReport> FaultReports { get; set; }
        public DbSet<AppUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AppUser>(entity =>
            {
                entity.HasIndex(user => user.Email)
                    .IsUnique();

                entity.Property(user => user.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(user => user.Email)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(user => user.Role)
                    .IsRequired()
                    .HasMaxLength(30)
                    .HasDefaultValue("Employee");
            });

            modelBuilder.Entity<FaultReport>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(report => report.AssignedTechnicianId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<FaultReport>()
                .HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(report => report.ReportedById)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}