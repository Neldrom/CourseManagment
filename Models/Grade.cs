namespace CourseManagment.Models
{
    public class Grade
    {
        public int GradeId { get; set; }
        public int EnrollmentId { get; set; }
        public int TeacherId { get; set; }
        public double Score { get; set; }
        public string Comments { get; set; }

        // Навигационни свойства
        public Enrollment Enrollment { get; set; }
        public ApplicationUser Teacher { get; set; }
    }
}
