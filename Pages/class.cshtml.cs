using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    // See Overview.cshtml.cs - reads User.GetStudentId()!.Value, so must
    // stay restricted to the Student role.
    [Authorize(Roles = "Student")]
    public class ClassModel : PageModel
    {
        private readonly AppDbContext _db;

        public ClassModel(AppDbContext db)
        {
            _db = db;
        }

        public string ClassName { get; set; } = "";

        public string SectionName { get; set; } = "";

        public List<ClassSubject> Subjects { get; set; } = new();

        // Shown inside every subject card. Not stored per-subject in the
        // database (yet) - every subject offers the same set of options for
        // now, matching the original hardcoded page.
        public List<ClassSubjectOption> SubjectOptions { get; set; } = new()
        {
            new ClassSubjectOption { Name = "Assignment", Icon = "" },
            new ClassSubjectOption { Name = "Quiz", Icon = "" },
            new ClassSubjectOption { Name = "Handouts", Icon = "" },
            new ClassSubjectOption { Name = "Teacher Remarks", Icon = "" },
            new ClassSubjectOption { Name = "Announcement", Icon = "" }
        };

        public async Task OnGetAsync()
        {
            var studentId = User.GetStudentId()!.Value;

            var student = await _db.Students
                .Include(s => s.ClassRoom)
                    .ThenInclude(c => c.Subjects)
                        .ThenInclude(cs => cs.Teacher)
                .FirstAsync(s => s.Id == studentId);

            ClassName = student.ClassRoom.ClassName;
            SectionName = student.ClassRoom.SectionName;

            Subjects = student.ClassRoom.Subjects
                .Select(cs => new ClassSubject
                {
                    Name = cs.Name,
                    TeacherName = cs.Teacher?.FullName ?? "Unassigned",
                    TeacherImageUrl = cs.Teacher?.PhotoData != null ? $"/image/teacher/{cs.Teacher.Id}" : ""
                })
                .ToList();
        }
    }


    // =============================================================
    // CLASS SUBJECT MODEL
    //
    // Named ClassSubject instead of Subject to avoid conflicts
    // with other Subject classes in the LMS project.
    //
    // NOTE: this is a separate view-model type from
    // Data.Entities.ClassSubject (the real database entity of almost the
    // same name) - kept distinct so this page's shape doesn't have to
    // change if the entity's does.
    // =============================================================

    public class ClassSubject
    {
        public string Name { get; set; } = "";

        public string TeacherName { get; set; } = "";

        public string TeacherImageUrl { get; set; } = "";
    }


    // =============================================================
    // SUBJECT OPTION MODEL
    // =============================================================

    public class ClassSubjectOption
    {
        public string Name { get; set; } = "";

        public string Icon { get; set; } = "";
    }
}
