using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages
{
    public class HomeModel : PageModel
    {
        // Dummy student info. Once authentication exists, this will come from
        // the logged-in user's session/claims instead of being hardcoded here.
        public string StudentName { get; set; } = "Ali Raza";
        public string RollNumber { get; set; } = "10-A-042";
        public string StudentInitials { get; set; } = "AR";

        // The five links shown inside every card. Defined once here instead of
        // repeated per subject in the markup.
        public List<string> SubjectOptions { get; } = new()
        {
            "Assignments", "Quizzes", "Announcements", "Handouts", "Teacher Remarks"
        };

        // Shaped as a List<Subject> on purpose: later we swap the dummy list
        // below for a real database query, and the Razor page won't need to
        // change at all since it just loops over whatever ends up in here.
        public List<Subject> Subjects { get; set; } = new();

        public void OnGet()
        {
            Subjects = new List<Subject>
            {
                new() { Name = "Science", TeacherName = "Mr. Ahmed", TeacherInitials = "MA" },
                new() { Name = "Mathematics", TeacherName = "Ms. Fatima", TeacherInitials = "MF" },
                new() { Name = "English", TeacherName = "Mrs. Khan", TeacherInitials = "MK" },
                new() { Name = "Art", TeacherName = "Mr. Bilal", TeacherInitials = "MB" },
                new() { Name = "Islamiyat", TeacherName = "Mr. Yousuf", TeacherInitials = "MY" },
                new() { Name = "Pakistan Studies", TeacherName = "Ms. Sana", TeacherInitials = "MS" },
                new() { Name = "Drawing", TeacherName = "Mr. Imran", TeacherInitials = "MI" },
                new() { Name = "Ethics", TeacherName = "Mrs. Noreen", TeacherInitials = "MN" },
            };
        }
    }

    // Plain C# class describing one subject. This will map directly to a
    // "Subjects" database table once Entity Framework is introduced.
    public class Subject
    {
        public string Name { get; set; } = "";
        public string TeacherName { get; set; } = "";
        public string TeacherInitials { get; set; } = "";
    }
}