using System.ComponentModel.DataAnnotations;

namespace CourseManagment.Models
{
    public class ProfileViewModel
    {

        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
