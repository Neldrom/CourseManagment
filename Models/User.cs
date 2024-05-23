using Microsoft.AspNetCore.Identity;

namespace CourseManagment.Models
{
    public class User : IdentityUser<int>
    {
        public string FullName { get; set; }

        // Навигационно свойство за ролите на потребителя
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Grade> GradesGiven { get; set; }  // Оценките, дадени от учителя
    }
}
