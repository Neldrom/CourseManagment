using System.Diagnostics;

namespace CourseManagment.Models
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public int UserId { get; set; }
        public int CourseId { get; set; }
        public DateTime EnrollmentDate { get; set; }

        // Навигационни свойства
        public User User { get; set; }
        public Course Course { get; set; }
        public ICollection<Grade> Grades { get; set; }
    }
}
