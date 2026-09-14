using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

namespace SchoolLMS.Pages.Admin.Students;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<StudentRow> Students { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        Students = await _db.Students
            .Include(s => s.ClassRoom)
            .OrderBy(s => s.ClassRoom.ClassName).ThenBy(s => s.RollNumber)
            .Select(s => new StudentRow(
                s.Id,
                s.FullName,
                s.RollNumber,
                s.ClassRoom.ClassName + " — " + s.ClassRoom.SectionName,
                _db.UserAccounts.Where(u => u.StudentId == s.Id).Select(u => u.Username).FirstOrDefault()))
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var student = await _db.Students.FindAsync(id);
        if (student is null)
        {
            return RedirectToPage();
        }

        // A login account left pointing at nothing would just be a dead,
        // confusing account - remove it along with the student rather
        // than relying on the FK's SetNull behavior to quietly orphan it.
        var linkedAccounts = await _db.UserAccounts.Where(u => u.StudentId == id).ToListAsync();
        _db.UserAccounts.RemoveRange(linkedAccounts);

        _db.Students.Remove(student);
        await _db.SaveChangesAsync();

        StatusMessage = $"Removed {student.FullName}.";
        return RedirectToPage();
    }
}

public record StudentRow(int Id, string FullName, string RollNumber, string ClassLabel, string? Username);
