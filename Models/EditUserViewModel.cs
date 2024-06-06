using System.ComponentModel.DataAnnotations;

namespace CourseManagment.Models
{
    public class EditUserViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(256)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(256)]
        public string Email { get; set; }
    }

}
