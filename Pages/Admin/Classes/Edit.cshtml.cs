using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
// SchoolLMS.Pages (the enclosing namespace) already defines a ClassSubject
// view-model record (in class.cshtml.cs), and SchoolLMS.Pages.Teacher is a
// real namespace (the Teacher portal pages) - both would otherwise shadow
// the real entity types here, since enclosing-namespace lookup wins over
// `using`.
using ClassSubjectEntity = SchoolLMS.Data.Entities.ClassSubject;
using TeacherEntity = SchoolLMS.Data.Entities.Teacher;

namespace SchoolLMS.Pages.Admin.Classes;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;

    public EditModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    [BindProperty]
    public string ClassName { get; set; } = "";

    [BindProperty]
    public string SectionName { get; set; } = "";

    [BindProperty]
    public string AcademicYear { get; set; } = "";

    [BindProperty]
    public string NewSubjectName { get; set; } = "";

    [BindProperty]
    public int? NewSubjectTeacherId { get; set; }

    public bool IsNew => Id is null or 0;

    public List<ClassSubjectEntity> Subjects { get; set; } = new();

    public List<TeacherEntity> AllTeachers { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        AllTeachers = await _db.Teachers.OrderBy(t => t.FullName).ToListAsync();

        if (!IsNew)
        {
            var classRoom = await _db.ClassRooms.FindAsync(Id);
            if (classRoom is null)
            {
                return RedirectToPage("Index");
            }

            ClassName = classRoom.ClassName;
            SectionName = classRoom.SectionName;
            AcademicYear = classRoom.AcademicYear;

            Subjects = await _db.ClassSubjects
                .Include(cs => cs.Teacher)
                .Where(cs => cs.ClassRoomId == Id)
                .OrderBy(cs => cs.Name)
                .ToListAsync();
        }

        return Page();
    }

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (string.IsNullOrWhiteSpace(ClassName) || string.IsNullOrWhiteSpace(SectionName) || string.IsNullOrWhiteSpace(AcademicYear))
        {
            ErrorMessage = "Class, section, and academic year are all required.";
            AllTeachers = await _db.Teachers.OrderBy(t => t.FullName).ToListAsync();
            return Page();
        }

        if (IsNew)
        {
            var classRoom = new ClassRoom
            {
                ClassName = ClassName,
                SectionName = SectionName,
                AcademicYear = AcademicYear
            };
            _db.ClassRooms.Add(classRoom);
            await _db.SaveChangesAsync();

            return RedirectToPage(new { id = classRoom.Id });
        }

        var existing = await _db.ClassRooms.FirstAsync(c => c.Id == Id);
        existing.ClassName = ClassName;
        existing.SectionName = SectionName;
        existing.AcademicYear = AcademicYear;
        await _db.SaveChangesAsync();

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostAddSubjectAsync()
    {
        if (string.IsNullOrWhiteSpace(NewSubjectName))
        {
            ErrorMessage = "Enter a subject name.";
        }
        else
        {
            _db.ClassSubjects.Add(new ClassSubjectEntity
            {
                ClassRoomId = Id!.Value,
                Name = NewSubjectName,
                TeacherId = NewSubjectTeacherId
            });
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostRemoveSubjectAsync(int classSubjectId)
    {
        var subject = await _db.ClassSubjects.FindAsync(classSubjectId);
        if (subject is not null)
        {
            _db.ClassSubjects.Remove(subject);
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = Id });
    }

    public async Task<IActionResult> OnPostUpdateTeacherAsync(int classSubjectId, int? teacherId)
    {
        var subject = await _db.ClassSubjects.FindAsync(classSubjectId);
        if (subject is not null)
        {
            subject.TeacherId = teacherId;
            await _db.SaveChangesAsync();
        }

        return RedirectToPage(new { id = Id });
    }
}
