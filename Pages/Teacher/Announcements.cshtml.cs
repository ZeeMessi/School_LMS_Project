using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class AnnouncementsModel : PageModel
{
    private readonly AppDbContext _db;

    public AnnouncementsModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string Title { get; set; } = "";

    [BindProperty]
    public string Category { get; set; } = "Notice";

    [BindProperty]
    public string ShortDescription { get; set; } = "";

    [BindProperty]
    public string FullDescription { get; set; } = "";

    [BindProperty]
    public bool IsImportant { get; set; }

    [BindProperty]
    public int TargetClassRoomId { get; set; }

    [BindProperty]
    public IFormFile? Attachment { get; set; }

    public List<ClassRoom> MyClasses { get; set; } = new();

    public List<Announcement> MyAnnouncements { get; set; } = new();

    public string? ErrorMessage { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var teacherId = User.GetTeacherId()!.Value;

        var ownsClass = await _db.ClassSubjects.AnyAsync(cs => cs.TeacherId == teacherId && cs.ClassRoomId == TargetClassRoomId);

        if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(ShortDescription) || !ownsClass)
        {
            ErrorMessage = "Title, a short description, and a class you actually teach are all required.";
            await LoadAsync();
            return Page();
        }

        var upload = await DocumentUploadHelper.ReadAsync(Attachment);
        if (upload.Error is not null)
        {
            ErrorMessage = upload.Error;
            await LoadAsync();
            return Page();
        }

        var teacher = await _db.Teachers.FirstAsync(t => t.Id == teacherId);

        _db.Announcements.Add(new Announcement
        {
            Title = Title,
            Category = Category,
            Date = DateTime.UtcNow,
            ShortDescription = ShortDescription,
            FullDescription = string.IsNullOrWhiteSpace(FullDescription) ? ShortDescription : FullDescription,
            IssuedBy = teacher.FullName,
            IsImportant = IsImportant,
            TargetClassRoomId = TargetClassRoomId,
            TeacherId = teacherId,
            AttachmentData = upload.Data,
            AttachmentContentType = upload.ContentType,
            AttachmentFileName = upload.FileName
        });
        await _db.SaveChangesAsync();

        StatusMessage = "Announcement posted.";
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var teacherId = User.GetTeacherId()!.Value;

        var announcement = await _db.Announcements.FirstOrDefaultAsync(a => a.Id == id && a.TeacherId == teacherId);
        if (announcement is not null)
        {
            _db.Announcements.Remove(announcement);
            await _db.SaveChangesAsync();
            StatusMessage = "Announcement removed.";
        }

        return RedirectToPage();
    }

    private async Task LoadAsync()
    {
        var teacherId = User.GetTeacherId()!.Value;

        MyClasses = await _db.ClassSubjects
            .Where(cs => cs.TeacherId == teacherId)
            .Select(cs => cs.ClassRoom)
            .Distinct()
            .ToListAsync();

        if (TargetClassRoomId == 0 && MyClasses.Count > 0)
        {
            TargetClassRoomId = MyClasses[0].Id;
        }

        MyAnnouncements = await _db.Announcements
            .Include(a => a.TargetClassRoom)
            .Where(a => a.TeacherId == teacherId)
            .OrderByDescending(a => a.Date)
            .ToListAsync();
    }
}
