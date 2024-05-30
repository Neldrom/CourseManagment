using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using CourseManagment.Data;
using CourseManagment.Models;

namespace CourseManagment.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var mostPopularCourses = await _context.Courses
                .Include(c => c.Enrollments)
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(3)
                .ToListAsync();

            var newlyAddedCourses = await _context.Courses
                .OrderByDescending(c => c.CourseId)
                .Take(3)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                MostPopularCourses = mostPopularCourses,
                NewlyAddedCourses = newlyAddedCourses
            };

            return View(viewModel);
        }
    }
}
