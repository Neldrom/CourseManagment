using Microsoft.AspNetCore.Identity;

namespace CourseManagment.Models
{
    public class Role : IdentityRole<int>
    {
        // Навигационно свойство за потребителите с тази роля
        public ICollection<UserRole> UserRoles { get; set; }
    }

}
