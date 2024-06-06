using CourseManagment.Data;
using CourseManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CourseManagment.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Home()
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.Courses
                .Where(c => c.TeacherId == user.Id)
                .ToListAsync();
            return View(courses);
        }

        [HttpGet]
        public IActionResult CreateCourse()
        {
            var model = new CreateCourseViewModel();
            DropdownHelper.PopulateDropdownLists(model);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCourseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var course = new Course
                {
                    Title = model.Title,
                    Description = model.Description,
                    Level = model.SelectedLevel,
                    Category = model.SelectedCategory,
                    TeacherId = user.Id
                };
                _context.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Home));
            }

            DropdownHelper.PopulateDropdownLists(model);

            return View(model);
        }

        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.User)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Grades)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(int enrollmentId, double grade)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Grades)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            var newGrade = new Grade
            {
                EnrollmentId = enrollmentId,
                TeacherId = enrollment.Course.TeacherId,
                GradeValue = grade
            };

            _context.Grades.Add(newGrade);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CourseDetails), new { id = enrollment.CourseId });
        }

        [HttpPost]
        public async Task<IActionResult> EditGrade(int gradeId, double newGradeValue)
        {
            var grade = await _context.Grades
                                      .Include(g => g.Enrollment)
                                      .ThenInclude(e => e.Course)
                                      .FirstOrDefaultAsync(g => g.GradeId == gradeId);

            if (grade == null)
            {
                return NotFound();
            }

            grade.GradeValue = newGradeValue;
            _context.Update(grade);
            await _context.SaveChangesAsync();

            return RedirectToAction("CourseDetails", new { id = grade.Enrollment.CourseId });
        }


        [HttpPost]
        public async Task<IActionResult> DeleteGrade(int gradeId)
        {
            var grade = await _context.Grades
                                      .Include(g => g.Enrollment)
                                      .ThenInclude(e => e.Course)
                                      .FirstOrDefaultAsync(g => g.GradeId == gradeId);
            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            return RedirectToAction("CourseDetails", new { id = grade.Enrollment.CourseId });
        }

    }
}
