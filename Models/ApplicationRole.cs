using Microsoft.AspNetCore.Identity;

namespace CourseManagment.Models
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        // Additional properties or constructors if needed
    }
}
