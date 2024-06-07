using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CourseManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using CourseManagment.Data;

namespace CourseManagment.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AccountController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Register()
        {
            var roles = await _roleManager.Roles
                .Where(r => r.Name != "Admin")
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToListAsync();

            var model = new RegisterViewModel
            {
                Roles = roles
            };

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register([Bind("UserName,Email,Password,ConfirmPassword,Role")] RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            model.Roles = await _roleManager.Roles
                .Where(r => r.Name != "Admin")
                .Select(r => new SelectListItem
                {
                    Value = r.Name,
                    Text = r.Name
                }).ToListAsync();

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);

                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var roles = await _userManager.GetRolesAsync(user);

                    return RedirectToAction("Index", "Home");
                    
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> Profile()
        {
            var userId = User.Identity.Name; 
            var user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            if (await _userManager.IsInRoleAsync(user, "Student"))
            {
                var enrollments = _context.Enrollments.Where(e => e.UserId == userId);
                var grades = enrollments.SelectMany(e => _context.Grades.Where(g => g.EnrollmentId == e.EnrollmentId).Select(g => g.GradeValue));
                ViewBag.AverageGrade = Math.Round(grades.Any() ? grades.Average() : 0, 2);
            }
            else if (await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                var courses = _context.Courses.Where(c => c.TeacherId == user.Id).ToList();
                var courseAverages = courses
                    .Where(c => _context.Enrollments
                                         .Where(e => e.CourseId == c.CourseId && e.Grades.Any())
                                         .SelectMany(e => e.Grades)
                                         .Any())
                    .Select(c => new
                    {
                        Course = c,
                        Average = Math.Round(_context.Enrollments
                                          .Where(e => e.CourseId == c.CourseId && e.Grades.Any())
                                          .SelectMany(e => e.Grades)
                                          .Average(g => g.GradeValue), 2)
                    }).ToList();

                ViewBag.CourseAverages = courseAverages;
                ViewBag.OverallAverage = Math.Round(courseAverages.Any() ? courseAverages.Average(c => c.Average) : 0, 2);
            }



            return View(user);
        }


        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userId); 
                if (user != null)
                {
                    var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
                    _context.SaveChanges();
                    return RedirectToAction("Profile");
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangeUsername()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeUsername(ChangeUsernameViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userId);
                if (user != null)
                {
                    var result = await _userManager.SetUserNameAsync(user, model.NewUsername);
                    if (result.Succeeded)
                    {
                        await _context.SaveChangesAsync();
                        await _signInManager.RefreshSignInAsync(user);

                        return RedirectToAction("Profile");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangeEmail()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangeEmail(ChangeEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.Name;
                var user = await _userManager.FindByNameAsync(userId);
                if (user != null)
                {
                    var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.NewEmail);
                    var result = await _userManager.ChangeEmailAsync(user, model.NewEmail, token);
                    _context.SaveChanges();
                    return RedirectToAction("Profile");
                }
            }
            return View(model);
        }
    }
}