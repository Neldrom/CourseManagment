using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;
using CourseManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace CourseManagment.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<ApplicationRole> roleManager, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
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

            _logger.LogInformation($"Roles found: {roles.Count}");
            foreach (var role in roles)
            {
                _logger.LogInformation($"Role: {role.Text}");
            }

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
                _logger.LogInformation("Model state is valid. Creating user.");
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created successfully. Adding to role.");
                    if (!string.IsNullOrEmpty(model.Role))
                    {
                        await _userManager.AddToRoleAsync(user, model.Role);
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"Error creating user: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                _logger.LogWarning("Model state is invalid.");
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"Model error: {modelError.ErrorMessage}");
                }
            }

            // Repopulate the roles if there's a return to the form, excluding Admin role
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

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
