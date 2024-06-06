using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagment.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        public double GradeValue { get; set; }

        public int EnrollmentId { get; set; }
        public Enrollment Enrollment { get; set; }

        public string TeacherId { get; set; }
        public ApplicationUser Teacher { get; set; }
    }
}
