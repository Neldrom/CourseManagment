using System.ComponentModel.DataAnnotations;

namespace CourseManagment.Models
{
    public class ProfileViewModel
    {
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
