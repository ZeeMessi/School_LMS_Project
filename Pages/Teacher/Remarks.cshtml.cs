using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;
// SchoolLMS.Pages (the enclosing namespace) already defines a ClassSubject
// view-model record (in class.cshtml.cs) that would otherwise shadow the
// real entity type here, since enclosing-namespace lookup wins over `using`.
using ClassSubjectEntity = SchoolLMS.Data.Entities.ClassSubject;

namespace SchoolLMS.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class RemarksModel : PageModel
{
    private readonly AppDbContext _db;

    public RemarksModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int ClassSubjectId { get; set; }

    [BindProperty]
    public int StudentId { get; set; }

    [BindProperty]
    public string Remark { get; set; } = "";

    public string ClassLabel { get; set; } = "";

    public string SubjectName { get; set; } = "";

    public List<Student> Students { get; set; } = new();

    public List<RemarkRow> Remarks { get; set; } = new();

    public string? ErrorMessage { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var classSubject = await EnsureTeacherOwnsSubjectAsync();
        if (classSubject is null)
        {
            return Forbid();
        }

        await LoadAsync(classSubject);
        return Page();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        var classSubject = await EnsureTeacherOwnsSubjectAsync();
        if (classSubject is null)
        {
            return Forbid();
        }

        var studentBelongsToClass = await _db.Students.AnyAsync(s => s.Id == StudentId && s.ClassRoomId == classSubject.ClassRoomId);

        if (string.IsNullOrWhiteSpace(Remark) || !studentBelongsToClass)
        {
            ErrorMessage = "Select a student and enter a remark.";
            await LoadAsync(classSubject);
            return Page();
        }

        _db.TeacherRemarks.Add(new TeacherRemark
        {
            StudentId = StudentId,
            ClassSubjectId = ClassSubjectId,
            Remark = Remark,
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();

        StatusMessage = "Remark added.";
        return RedirectToPage(new { ClassSubjectId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var classSubject = await EnsureTeacherOwnsSubjectAsync();
        if (classSubject is null)
        {
            return Forbid();
        }

        var remark = await _db.TeacherRemarks.FirstOrDefaultAsync(r => r.Id == id && r.ClassSubjectId == ClassSubjectId);
        if (remark is not null)
        {
            _db.TeacherRemarks.Remove(remark);
            await _db.SaveChangesAsync();
            StatusMessage = "Remark removed.";
        }

        return RedirectToPage(new { ClassSubjectId });
    }

    private async Task<ClassSubjectEntity?> EnsureTeacherOwnsSubjectAsync()
    {
        var teacherId = User.GetTeacherId()!.Value;

        return await _db.ClassSubjects
            .Include(cs => cs.ClassRoom)
            .FirstOrDefaultAsync(cs => cs.Id == ClassSubjectId && cs.TeacherId == teacherId);
    }

    private async Task LoadAsync(ClassSubjectEntity classSubject)
    {
        ClassLabel = $"{classSubject.ClassRoom.ClassName} — {classSubject.ClassRoom.SectionName}";
        SubjectName = classSubject.Name;

        Students = await _db.Students
            .Where(s => s.ClassRoomId == classSubject.ClassRoomId)
            .OrderBy(s => s.RollNumber)
            .ToListAsync();

        Remarks = await _db.TeacherRemarks
            .Include(r => r.Student)
            .Where(r => r.ClassSubjectId == ClassSubjectId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new RemarkRow(r.Id, r.Student.FullName, r.Remark, r.CreatedAt))
            .ToListAsync();
    }
}

public record RemarkRow(int Id, string StudentName, string Remark, DateTime CreatedAt);
