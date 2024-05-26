using Microsoft.AspNetCore.Mvc;
using CourseManagment.Data;

public class HomeController : Controller
{
    private readonly ApplicationDbContext _context;

    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var mostPopularCourses = _context.Courses
            .OrderByDescending(c => c.Enrollments.Count)
            .Take(3)
            .ToList();

        return View(mostPopularCourses);
    }
}
