using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

namespace SchoolLMS.Pages.Admin.Feedback;

// Read side of Pages/Feedback.cshtml (the student-facing submission
// form) - every submission, with enough of the student's identity
// (name, class/section) for Admin to actually act on it.
[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    public List<FeedbackRow> Submissions { get; set; } = new();

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int id, string status)
    {
        var submission = await _db.FeedbackSubmissions.FirstOrDefaultAsync(f => f.Id == id);
        if (submission is not null && (status is "New" or "Reviewed" or "Resolved"))
        {
            submission.Status = status;
            await _db.SaveChangesAsync();
            StatusMessage = "Status updated.";
        }

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var submissions = await _db.FeedbackSubmissions
            .Include(f => f.Student)
                .ThenInclude(s => s!.ClassRoom)
            .OrderByDescending(f => f.SubmittedAt)
            .ToListAsync();

        Submissions = submissions
            .Select(f => new FeedbackRow(
                f.Id,
                f.Student?.FullName ?? "Unknown student",
                f.Student is null ? "" : $"{f.Student.ClassRoom.ClassName} — {f.Student.ClassRoom.SectionName}",
                f.Category,
                f.Rating,
                f.Message,
                f.SubmittedAt,
                f.Status))
            .ToList();
    }
}

public record FeedbackRow(
    int Id,
    string StudentName,
    string ClassLabel,
    string Category,
    int Rating,
    string Message,
    DateTime SubmittedAt,
    string Status);
