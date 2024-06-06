using CourseManagment.Data;
using CourseManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CourseManagment.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Home()
        {
            var user = await _userManager.GetUserAsync(User);
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.UserId == user.Id)
                .Select(e => e.CourseId)
                .ToListAsync();

            var courses = await _context.Courses
                .Where(c => enrolledCourseIds.Contains(c.CourseId))
                .ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Courses()
        {
            var user = await _userManager.GetUserAsync(User);
            var enrolledCourseIds = await _context.Enrollments
                .Where(e => e.UserId == user.Id)
                .Select(e => e.CourseId)
                .ToListAsync();

            var courses = await _context.Courses
                .Where(c => !enrolledCourseIds.Contains(c.CourseId))
                .ToListAsync();

            return View(courses);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            var existingEnrollment = await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.UserId == user.Id);

            if (existingEnrollment)
            {
                ModelState.AddModelError(string.Empty, "You are already enrolled in this course.");
                return RedirectToAction(nameof(Courses));
            }

            var enrollment = new Enrollment
            {
                CourseId = courseId,
                UserId = user.Id
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Home));
        }

        public async Task<IActionResult> CourseDetails(int courseId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.User)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Grades)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

    }
}
