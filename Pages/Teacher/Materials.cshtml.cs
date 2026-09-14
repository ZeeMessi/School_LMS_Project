using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
public class MaterialsModel : PageModel
{
    private readonly AppDbContext _db;

    public MaterialsModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int ClassSubjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public MaterialType Type { get; set; } = MaterialType.Assignment;

    [BindProperty]
    public string Title { get; set; } = "";

    [BindProperty]
    public string Description { get; set; } = "";

    [BindProperty]
    public DateOnly? DueDate { get; set; }

    // Named Attachment, not File - PageModel already has a File(...) method
    // for returning file results, which a same-named property would hide.
    [BindProperty]
    public IFormFile? Attachment { get; set; }

    public string ClassLabel { get; set; } = "";

    public string SubjectName { get; set; } = "";

    public string TypePlural => Type == MaterialType.Quiz ? "Quizzes" : $"{Type}s";

    public List<CourseMaterial> Materials { get; set; } = new();

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

        if (string.IsNullOrWhiteSpace(Title))
        {
            ErrorMessage = "A title is required.";
            await LoadAsync(classSubject);
            return Page();
        }

        var upload = await DocumentUploadHelper.ReadAsync(Attachment);
        if (upload.Error is not null)
        {
            ErrorMessage = upload.Error;
            await LoadAsync(classSubject);
            return Page();
        }

        _db.CourseMaterials.Add(new CourseMaterial
        {
            ClassSubjectId = ClassSubjectId,
            Type = Type,
            Title = Title,
            Description = Description,
            DueDate = DueDate,
            PostedAt = DateTime.UtcNow,
            FileData = upload.Data,
            FileContentType = upload.ContentType,
            FileName = upload.FileName
        });
        await _db.SaveChangesAsync();

        StatusMessage = $"{Type} posted.";
        return RedirectToPage(new { ClassSubjectId, Type });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var classSubject = await EnsureTeacherOwnsSubjectAsync();
        if (classSubject is null)
        {
            return Forbid();
        }

        var material = await _db.CourseMaterials.FirstOrDefaultAsync(m => m.Id == id && m.ClassSubjectId == ClassSubjectId);
        if (material is not null)
        {
            _db.CourseMaterials.Remove(material);
            await _db.SaveChangesAsync();
            StatusMessage = "Removed.";
        }

        return RedirectToPage(new { ClassSubjectId, Type });
    }

    // Confirms the logged-in teacher is actually assigned to this
    // ClassSubject, rather than trusting the classSubjectId query
    // parameter outright - same pattern as Attendance/Grades.
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

        Materials = await _db.CourseMaterials
            .Where(m => m.ClassSubjectId == ClassSubjectId && m.Type == Type)
            .OrderByDescending(m => m.PostedAt)
            .ToListAsync();
    }
}
