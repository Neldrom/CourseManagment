using System.ComponentModel.DataAnnotations;

namespace CourseManagment.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current Password")]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }
    }

    public class ChangeUsernameViewModel
    {
        [Required]
        [Display(Name = "New Username")]
        public string NewUsername { get; set; }
    }

    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "New Email")]
        public string NewEmail { get; set; }
    }

}
