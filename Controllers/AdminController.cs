using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using CourseManagment.Data;
using CourseManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CourseManagment.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Users()
        {
            var adminRole = await _context.Roles.SingleOrDefaultAsync(r => r.Name == "Admin");
            var users = await _userManager.Users.ToListAsync();
            var nonAdminUsers = new List<ApplicationUser>();

            foreach (var user in users)
            {
                if (!await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    nonAdminUsers.Add(user);
                }
            }

            return View(nonAdminUsers);
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser(ApplicationUser user)
        {
            if (ModelState.IsValid)
            {
                var result = await _userManager.CreateAsync(user, "DefaultPassword123!");
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Users));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> EditUser(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();
            }

            user.UserName = model.UserName;
            user.Email = model.Email;

            try
            {
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Users));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(model.Id))
                {
                    return NotFound();
                }
                else
                {
                    ModelState.AddModelError("", "Unable to save changes. The user was updated or deleted by another user.");
                    return View(model);
                }
            }

            return View(model);
        }

        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (id == null) return NotFound();
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("DeleteUser")]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Users));
        }

        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses
                .Include(c => c.Teacher)
                .ToListAsync();
            return View(courses);
        }


        public IActionResult CreateCourse()
        {
            var teachers = _userManager.Users
                .ToList()
                .Where(u => _userManager.IsInRoleAsync(u, "Teacher").Result)
                .ToList();

            Console.WriteLine($"Found {teachers.Count} teachers.");

            ViewBag.Teachers = new SelectList(teachers, "Id", "UserName");

            var model = new CreateCourseViewModelAdmin();
            DropdownHelper.PopulateDropdownLists(model);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCourse(CreateCourseViewModelAdmin course)
        {
            if (ModelState.IsValid)
            {
                var newCourse = new Course
                {
                    Title = course.Title,
                    TeacherId = course.TeacherId,
                    Description = course.Description,
                    Level = course.SelectedLevel,
                    Category = course.SelectedCategory
                };
                _context.Add(newCourse);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }

            var teachers = _userManager.Users
                .Where(u => _userManager.IsInRoleAsync(u, "Teacher").Result)
                .ToList();

            ViewBag.Teachers = new SelectList(teachers, "Id", "UserName");

            return View(course);
        }



        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            var users = _userManager.Users.ToList();
            var teachers = users.Where(u => _userManager.IsInRoleAsync(u, "Teacher").Result).ToList();

            ViewBag.Teachers = new SelectList(teachers, "Id", "UserName");

            var model = new CreateCourseViewModelAdmin
            {
                CourseId = course.CourseId,
                Title = course.Title,
                Description = course.Description,
                SelectedLevel = course.Level,
                SelectedCategory = course.Category,
                TeacherId = course.TeacherId
            };
            DropdownHelper.PopulateDropdownLists(model);
            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> EditCourse(CreateCourseViewModelAdmin model)
        {
            if (ModelState.IsValid)
            {
                var course = await _context.Courses.FindAsync(model.CourseId);
                if (course == null)
                {
                    return NotFound();
                }

                course.Title = model.Title;
                course.Description = model.Description;
                course.Level = model.SelectedLevel;
                course.Category = model.SelectedCategory;
                course.TeacherId = model.TeacherId;

                _context.Update(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }

            var teachers = _userManager.Users
                .Where(u => _userManager.IsInRoleAsync(u, "Teacher").Result)
                .ToList();

            ViewBag.Teachers = new SelectList(teachers, "Id", "UserName");
            DropdownHelper.PopulateDropdownLists(model); 

            return View(model);
        }



        [HttpGet]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }
        [HttpPost, ActionName("DeleteCourse")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourseConfirmed(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Grades)
                .FirstOrDefaultAsync(c => c.CourseId == id);

            if (course == null)
            {
                return NotFound();
            }

            foreach (var enrollment in course.Enrollments.ToList())
            {
                foreach (var grade in enrollment.Grades.ToList())
                {
                    _context.Grades.Remove(grade);
                }
                _context.Enrollments.Remove(enrollment);
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Courses));
        }

        public async Task<IActionResult> Enrollments()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .ToListAsync();

            var groupedEnrollments = enrollments
                .GroupBy(e => e.Course)
                .ToList();

            return View(groupedEnrollments);
        }



        [HttpGet]
        public async Task<IActionResult> CreateEnrollment()
        {
            var users = await _userManager.Users.ToListAsync();

            var nonAdminAndTeacherUsers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin") && !roles.Contains("Teacher"))
                {
                    nonAdminAndTeacherUsers.Add(user);
                }
            }

            var courses = await _context.Courses.ToListAsync();

            var model = new CreateEnrollmentViewModel
            {
                Users = new SelectList(nonAdminAndTeacherUsers, "Id", "UserName"),
                Courses = new SelectList(courses, "CourseId", "Title")
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEnrollment(CreateEnrollmentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var enrollment = new Enrollment
                {
                    UserId = model.SelectedUserId,
                    CourseId = model.SelectedCourseId
                };

                _context.Enrollments.Add(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Enrollments));
            }

            var users = await _userManager.Users.ToListAsync();
            var nonAdminAndTeacherUsers = new List<ApplicationUser>();
            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin") && !roles.Contains("Teacher"))
                {
                    nonAdminAndTeacherUsers.Add(user);
                }
            }

            var courses = await _context.Courses.ToListAsync();
            model.Users = new SelectList(nonAdminAndTeacherUsers, "Id", "UserName");
            model.Courses = new SelectList(courses, "CourseId", "Title");

            return View(model);
        }



        public async Task<IActionResult> EditEnrollment(int id)
        {
            if (id == 0) return NotFound();
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null) return NotFound();
            return View(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> EditEnrollment(Enrollment enrollment)
        {
            if (ModelState.IsValid)
            {
                _context.Update(enrollment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Enrollments));
            }
            return View(enrollment);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [HttpPost, ActionName("DeleteEnrollment")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteEnrollmentConfirmed(int id)
        {
            var enrollment = await _context.Enrollments.FindAsync(id);
            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Enrollments));
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGrade(int EnrollmentId, double GradeValue, string TeacherId)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.Grades)
                .FirstOrDefaultAsync(e => e.EnrollmentId == EnrollmentId);

            if (enrollment == null)
            {
                return NotFound();
            }

            var grade = new Grade
            {
                GradeValue = GradeValue,
                EnrollmentId = EnrollmentId,
                TeacherId = TeacherId
            };

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(EditGrades), new { id = EnrollmentId });
        }




        public async Task<IActionResult> EditGrades(int id)
        {
            var enrollment = await _context.Enrollments
                .Include(e => e.User)
                .Include(e => e.Course)
                .Include(e => e.Grades)
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        [HttpPost]
        public async Task<IActionResult> EditGrades(int id, List<Grade> grades)
        {
            
                var enrollment = await _context.Enrollments
                    .Include(e => e.Grades)
                    .FirstOrDefaultAsync(e => e.EnrollmentId == id);

                if (enrollment == null)
                {
                    return NotFound();
                }

                foreach (var grade in grades)
                {
                    var existingGrade = enrollment.Grades.FirstOrDefault(g => g.GradeId == grade.GradeId);
                    if (existingGrade != null)
                    {
                        existingGrade.GradeValue = grade.GradeValue;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Enrollments));
            }




        [HttpGet]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var grade = await _context.Grades
                .Include(g => g.Enrollment)
                .ThenInclude(e => e.User)
                .Include(g => g.Enrollment)
                .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(g => g.GradeId == id);

            if (grade == null)
            {
                return NotFound();
            }

            return View(grade);
        }

        [HttpPost, ActionName("DeleteGrade")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGradeConfirmed(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null)
            {
                return NotFound();
            }

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Enrollments));
        }
    }
}
