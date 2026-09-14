using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

namespace SchoolLMS.Pages.Admin.Classes;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<ClassRow> Classes { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    [TempData]
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        Classes = await _db.ClassRooms
            .OrderBy(c => c.ClassName).ThenBy(c => c.SectionName)
            .Select(c => new ClassRow(
                c.Id,
                c.ClassName,
                c.SectionName,
                c.AcademicYear,
                c.Students.Count,
                c.Subjects.Count))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var classRoom = await _db.ClassRooms.Include(c => c.Students).FirstOrDefaultAsync(c => c.Id == id);
        if (classRoom is null)
        {
            return RedirectToPage();
        }

        if (classRoom.Students.Count > 0)
        {
            ErrorMessage = $"Can't delete {classRoom.ClassName} — {classRoom.SectionName}: it still has {classRoom.Students.Count} student(s) enrolled. Move or remove them first.";
            return RedirectToPage();
        }

        _db.ClassRooms.Remove(classRoom);
        await _db.SaveChangesAsync();

        StatusMessage = $"Removed {classRoom.ClassName} — {classRoom.SectionName}.";
        return RedirectToPage();
    }
}

public record ClassRow(int Id, string ClassName, string SectionName, string AcademicYear, int StudentCount, int SubjectCount);
