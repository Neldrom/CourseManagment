using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CourseManagment.Models
{
    public class CreateCourseViewModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Display(Name = "Course Level")]
        public string SelectedLevel { get; set; }
        [ValidateNever]

        public IEnumerable<SelectListItem> Levels { get; set; }

        [Display(Name = "Category")]
        public string SelectedCategory { get; set; }
        [ValidateNever]

        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
