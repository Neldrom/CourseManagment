using Microsoft.AspNetCore.Identity;

namespace CourseManagment.Models
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Grade> GradesGiven { get; set; }  // Оценките, дадени от учителя
    }
}
