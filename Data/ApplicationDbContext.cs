using CourseManagment.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseManagment.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Configure User-Course relationship
            builder.Entity<Course>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete

            // Configure Enrollment relationships
            builder.Entity<Enrollment>()
                .HasOne(e => e.User)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete

            // Configure Grade relationships
            builder.Entity<Grade>()
                .HasOne(g => g.Enrollment)
                .WithMany(e => e.Grades)
                .HasForeignKey(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict); // No cascade delete

            builder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany()
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.NoAction); // No cascade delete
        }
    }
}
