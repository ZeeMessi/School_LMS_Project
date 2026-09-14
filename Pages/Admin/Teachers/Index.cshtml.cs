using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Services;

namespace SchoolLMS.Pages.Admin.Teachers;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<TeacherRow> Teachers { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var teachers = await _db.Teachers
            .Include(t => t.TaughtSubjects)
                .ThenInclude(cs => cs.ClassRoom)
            .OrderBy(t => t.FullName)
            .ToListAsync();

        var usernames = await _db.UserAccounts
            .Where(u => u.TeacherId != null)
            .ToDictionaryAsync(u => u.TeacherId!.Value, u => u.Username);

        Teachers = teachers
            .Select(t => new TeacherRow(
                t.Id,
                t.FullName,
                t.PhotoData != null ? $"/image/teacher/{t.Id}" : AvatarHelper.PlaceholderDataUri(t.Gender),
                usernames.GetValueOrDefault(t.Id),
                t.TaughtSubjects.Count == 0
                    ? "No subjects assigned"
                    : string.Join(", ", t.TaughtSubjects.Select(cs => $"{cs.Name} ({cs.ClassRoom.ClassName} — {cs.ClassRoom.SectionName})"))))
            .ToList();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var teacher = await _db.Teachers.FindAsync(id);
        if (teacher is null)
        {
            return RedirectToPage();
        }

        var linkedAccounts = await _db.UserAccounts.Where(u => u.TeacherId == id).ToListAsync();
        _db.UserAccounts.RemoveRange(linkedAccounts);

        // ClassSubject.TeacherId is SetNull on delete (see AppDbContext),
        // so removing a teacher un-assigns them from their subjects rather
        // than deleting the subjects themselves.
        _db.Teachers.Remove(teacher);
        await _db.SaveChangesAsync();

        StatusMessage = $"Removed {teacher.FullName}.";
        return RedirectToPage();
    }
}

public record TeacherRow(int Id, string FullName, string AvatarUrl, string? Username, string SubjectsSummary);
