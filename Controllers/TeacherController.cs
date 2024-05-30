using CourseManagment.Data;
using CourseManagment.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

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

        // GET: Teacher/Home
        public async Task<IActionResult> Home()
        {
            var user = await _userManager.GetUserAsync(User);
            var courses = await _context.Courses
                .Where(c => c.TeacherId == user.Id)
                .ToListAsync();
            return View(courses);
        }

        // GET: Teacher/CreateCourse
        [HttpGet]
        public IActionResult CreateCourse()
        {
            var model = new CreateCourseViewModel
            {
                Levels = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Beginner", Text = "Beginner" },
                    new SelectListItem { Value = "Intermediate", Text = "Intermediate" },
                    new SelectListItem { Value = "Advanced", Text = "Advanced" },
                    new SelectListItem { Value = "Expert", Text = "Expert" }
                },
                Categories = new List<SelectListItem>
                {
                    new SelectListItem { Value = "Maths", Text = "Maths" },
                    new SelectListItem { Value = "Physics", Text = "Physics" },
                    new SelectListItem { Value = "Chemistry", Text = "Chemistry" },
                    new SelectListItem { Value = "Biology", Text = "Biology" },
                    new SelectListItem { Value = "Computer Science", Text = "Computer Science" },
                    new SelectListItem { Value = "Literature", Text = "Literature" },
                    new SelectListItem { Value = "History", Text = "History" },
                    new SelectListItem { Value = "Art", Text = "Art" },
                    new SelectListItem { Value = "Music", Text = "Music" },
                    new SelectListItem { Value = "Economics", Text = "Economics" }
                }
            };

            return View(model);
        }

        // POST: Teacher/CreateCourse
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

            // Repopulate dropdown lists if the model state is invalid
            model.Levels = new List<SelectListItem>
            {
                new SelectListItem { Value = "Beginner", Text = "Beginner" },
                new SelectListItem { Value = "Intermediate", Text = "Intermediate" },
                new SelectListItem { Value = "Advanced", Text = "Advanced" },
                new SelectListItem { Value = "Expert", Text = "Expert" }
            };
            model.Categories = new List<SelectListItem>
            {
                new SelectListItem { Value = "Maths", Text = "Maths" },
                new SelectListItem { Value = "Physics", Text = "Physics" },
                new SelectListItem { Value = "Chemistry", Text = "Chemistry" },
                new SelectListItem { Value = "Biology", Text = "Biology" },
                new SelectListItem { Value = "Computer Science", Text = "Computer Science" },
                new SelectListItem { Value = "Literature", Text = "Literature" },
                new SelectListItem { Value = "History", Text = "History" },
                new SelectListItem { Value = "Art", Text = "Art" },
                new SelectListItem { Value = "Music", Text = "Music" },
                new SelectListItem { Value = "Economics", Text = "Economics" }
            };

            return View(model);
        }

        // GET: Teacher/CourseDetails/5
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

        // POST: Teacher/AddGrade
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(int enrollmentId, string grade)
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

        // POST: Teacher/EditGrade
        [HttpPost]
        public async Task<IActionResult> EditGrade(int gradeId, string newGradeValue)
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


        // POST: Teacher/DeleteGrade
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
