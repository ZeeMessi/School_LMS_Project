using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages;

// One shared page for the four "Assignment / Quiz / Handouts / Teacher
// Remarks" buttons on class.cshtml's subject cards, instead of four
// near-identical pages - see Overview.cshtml.cs for why this stays
// restricted to the Student role.
[Authorize(Roles = "Student")]
public class SubjectContentModel : PageModel
{
    private readonly AppDbContext _db;

    public SubjectContentModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int ClassSubjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string Section { get; set; } = "assignments";

    public string SubjectName { get; set; } = "";

    public string TeacherName { get; set; } = "";

    public string ClassLabel { get; set; } = "";

    public List<CourseMaterial> Assignments { get; set; } = new();

    public List<CourseMaterial> Handouts { get; set; } = new();

    public List<CourseMaterial> Quizzes { get; set; } = new();

    public List<TeacherRemark> Remarks { get; set; } = new();

    public async Task<IActionResult> OnGetAsync()
    {
        var studentId = User.GetStudentId()!.Value;

        var student = await _db.Students.Include(s => s.ClassRoom).FirstAsync(s => s.Id == studentId);

        // The classSubjectId in the URL isn't trusted outright - confirm
        // it actually belongs to this student's own class before showing
        // anything, the same way the Teacher pages confirm ownership
        // before showing theirs.
        var classSubject = await _db.ClassSubjects
            .Include(cs => cs.Teacher)
            .Include(cs => cs.ClassRoom)
            .FirstOrDefaultAsync(cs => cs.Id == ClassSubjectId && cs.ClassRoomId == student.ClassRoomId);

        if (classSubject is null)
        {
            return Forbid();
        }

        SubjectName = classSubject.Name;
        TeacherName = classSubject.Teacher?.FullName ?? "Unassigned";
        ClassLabel = $"{classSubject.ClassRoom.ClassName} — {classSubject.ClassRoom.SectionName}";

        var materials = await _db.CourseMaterials
            .Where(m => m.ClassSubjectId == ClassSubjectId)
            .OrderByDescending(m => m.PostedAt)
            .ToListAsync();

        Assignments = materials.Where(m => m.Type == MaterialType.Assignment).ToList();
        Handouts = materials.Where(m => m.Type == MaterialType.Handout).ToList();
        Quizzes = materials.Where(m => m.Type == MaterialType.Quiz).ToList();

        Remarks = await _db.TeacherRemarks
            .Where(r => r.ClassSubjectId == ClassSubjectId && r.StudentId == studentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();

        return Page();
    }
}
