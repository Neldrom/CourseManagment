using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagment.Models
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Level { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string TeacherId { get; set; }
        public ApplicationUser Teacher { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
    }
}
