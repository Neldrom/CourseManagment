using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

public class ApplicationUser : IdentityUser<int>
{
    // Additional properties related to ApplicationUser
    public ICollection<Enrollment> Enrollments { get; set; }

    // Assuming the user can give grades, add this property
    public ICollection<Grade> GradesGiven { get; set; }

    public ApplicationUser()
    {
        Enrollments = new HashSet<Enrollment>();
        GradesGiven = new HashSet<Grade>();
    }
}
