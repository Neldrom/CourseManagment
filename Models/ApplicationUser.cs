using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CourseManagment.Models
{
    public class ApplicationUser : IdentityUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public override string Id { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }
        public ICollection<Grade> GradesGiven { get; set; }

        public ApplicationUser()
        {
            Enrollments = new HashSet<Enrollment>();
            GradesGiven = new HashSet<Grade>();
        }
    }
}
