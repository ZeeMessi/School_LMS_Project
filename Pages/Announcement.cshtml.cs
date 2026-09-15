using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    // See Overview.cshtml.cs - reads User.GetStudentId()!.Value, so must
    // stay restricted to the Student role.
    [Authorize(Roles = "Student")]
    public class AnnouncementModel : PageModel
    {
        private readonly AppDbContext _db;

        public AnnouncementModel(AppDbContext db)
        {
            _db = db;
        }

        public List<AnnouncementItem> Announcements { get; set; } = new();

        public async Task OnGetAsync()
        {
            // "Read" tracking (AnnouncementRead) isn't wired up yet - every
            // announcement shows as unread for now, rather than
            // fabricating a read/unread split.
            var studentId = User.GetStudentId()!.Value;
            var student = await _db.Students.FirstAsync(s => s.Id == studentId);

            var announcements = await _db.Announcements
                .Where(a => a.TargetClassRoomId == null || a.TargetClassRoomId == student.ClassRoomId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();

            Announcements = announcements
                .Select(a => new AnnouncementItem
                {
                    Id = a.Id,
                    Title = a.Title,
                    Category = a.Category,
                    Date = a.Date,
                    ShortDescription = a.ShortDescription,
                    FullDescription = a.FullDescription,
                    IssuedBy = a.IssuedBy,
                    IsImportant = a.IsImportant,
                    IsRead = false,
                    AttachmentUrl = a.AttachmentData is null ? null : $"/file/announcement/{a.Id}",
                    AttachmentName = a.AttachmentFileName
                })
                .ToList();

            await NavTrackingService.MarkViewedAsync(_db, studentId, NavSection.Announcement);
        }
    }


    /*
     * ============================================================
     * ANNOUNCEMENT DATA MODEL
     * ============================================================
     *
     * These properties are intentionally separated so that the
     * database can provide them later without changing the UI.
     */

    public class AnnouncementItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string ShortDescription { get; set; } = string.Empty;

        public string FullDescription { get; set; } = string.Empty;

        public string IssuedBy { get; set; } = string.Empty;

        public bool IsImportant { get; set; }

        public bool IsRead { get; set; }

        public string? AttachmentUrl { get; set; }

        public string? AttachmentName { get; set; }
    }
}
