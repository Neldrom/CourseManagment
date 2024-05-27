using CourseManagment.Models;
public class Grade
{
    public int GradeId { get; set; }
    public int EnrollmentId { get; set; }
    public Enrollment Enrollment { get; set; }
    public int TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; }
    // Other properties
}
