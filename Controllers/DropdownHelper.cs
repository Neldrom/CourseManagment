using CourseManagment.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

public static class DropdownHelper
{
    public static void PopulateDropdownLists(CreateCourseViewModel model)
    {
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
    }

    public static void PopulateDropdownLists(CreateCourseViewModelAdmin model)
    {
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
    }
}
