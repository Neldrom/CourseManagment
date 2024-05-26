using CourseManagment.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CourseManagment.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }  // Ensure this line is present


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Конфигурация на релациите между потребители и роли
            
           

            // Конфигурация на релациите за Enrollment
            builder.Entity<Enrollment>()
                .HasOne(e => e.ApplicationUser)
                .WithMany(u => u.Enrollments)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Премахване на каскадното изтриване

            builder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // Премахване на каскадното изтриване

            // Конфигурация на релациите за Grade
            builder.Entity<Grade>()
                .HasOne(g => g.Enrollment)
                .WithMany(e => e.Grades)
                .HasForeignKey(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Restrict); // Премахване на каскадното изтриване

            builder.Entity<Grade>()
                .HasOne(g => g.Teacher)
                .WithMany(t => t.GradesGiven)
                .HasForeignKey(g => g.TeacherId)
                .OnDelete(DeleteBehavior.Restrict); // Премахване на каскадното изтриване
        }
    }
}
