namespace CourseManagment.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string Level { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Instructor { get; set; }

        // Навигационно свойство за записванията
        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
