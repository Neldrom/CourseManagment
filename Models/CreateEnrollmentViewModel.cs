using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseManagment.Models
{
    public class CreateEnrollmentViewModel
    {
        [Required]
        public string SelectedUserId { get; set; }

        [Required]
        public int SelectedCourseId { get; set; }

        public IEnumerable<SelectListItem> Users { get; set; }
        public IEnumerable<SelectListItem> Courses { get; set; }
    }
}
