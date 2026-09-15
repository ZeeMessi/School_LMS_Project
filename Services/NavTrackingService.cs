using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Services;

// Powers the "new" dot on the student sidebar's Announcement/Exam
// Schedule/Grade Book/Account Book/Class/Overview links (see
// Pages/Shared/_Layout.cshtml) - the whole-tab counterpart to
// ContentTrackingService's per-subject boxes/tabs. Class and Overview
// are pure aggregates (see NavFlags.Class/Overview) rather than having
// their own StudentNavView row, since there's nothing on either page
// itself to mark "viewed" beyond what's already tracked elsewhere.
public static class NavTrackingService
{
    public static async Task<NavFlags> GetNavFlagsAsync(AppDbContext db, int studentId, int classRoomId)
    {
        var classSubjects = await db.ClassSubjects
            .Where(cs => cs.ClassRoomId == classRoomId)
            .ToListAsync();

        var subjectFlags = await ContentTrackingService.GetNewFlagsAsync(db, studentId, classRoomId, classSubjects);
        var classHasNew = subjectFlags.Values.Any(v => v);

        var announcementLatest = await db.Announcements
            .Where(a => a.TargetClassRoomId == null || a.TargetClassRoomId == classRoomId)
            .Select(a => (DateTime?)a.Date)
            .OrderByDescending(d => d)
            .FirstOrDefaultAsync();

        var examScheduleLatest = await db.Exams
            .Where(e => e.ClassRoomId == classRoomId)
            .Select(e => (DateTime?)e.CreatedAt)
            .OrderByDescending(d => d)
            .FirstOrDefaultAsync();

        var gradeBookLatest = await db.ExamResults
            .Where(r => r.StudentId == studentId)
            .Select(r => (DateTime?)r.RecordedAt)
            .OrderByDescending(d => d)
            .FirstOrDefaultAsync();

        var accountBookLatest = await db.FeeInvoices
            .Where(f => f.StudentId == studentId)
            .Select(f => (DateOnly?)f.IssueDate)
            .OrderByDescending(d => d)
            .FirstOrDefaultAsync();
        var accountBookLatestDateTime = accountBookLatest?.ToDateTime(TimeOnly.MinValue);

        var views = await db.StudentNavViews
            .Where(v => v.StudentId == studentId)
            .ToListAsync();
        var viewLookup = views.ToDictionary(v => v.Section, v => v.LastViewedAt);

        bool HasNew(NavSection section, DateTime? latest)
        {
            if (latest is null)
            {
                return false;
            }

            return !viewLookup.TryGetValue(section, out var lastViewed) || latest > lastViewed;
        }

        return new NavFlags
        {
            Announcement = HasNew(NavSection.Announcement, announcementLatest),
            ExamSchedule = HasNew(NavSection.ExamSchedule, examScheduleLatest),
            GradeBook = HasNew(NavSection.GradeBook, gradeBookLatest),
            AccountBook = HasNew(NavSection.AccountBook, accountBookLatestDateTime),
            Class = classHasNew
        };
    }

    public static async Task MarkViewedAsync(AppDbContext db, int studentId, NavSection section)
    {
        var existing = await db.StudentNavViews.FirstOrDefaultAsync(v =>
            v.StudentId == studentId && v.Section == section);

        if (existing is null)
        {
            db.StudentNavViews.Add(new StudentNavView
            {
                StudentId = studentId,
                Section = section,
                LastViewedAt = DateTime.UtcNow
            });
        }
        else
        {
            existing.LastViewedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();
    }

    public class NavFlags
    {
        public bool Announcement { get; set; }
        public bool ExamSchedule { get; set; }
        public bool GradeBook { get; set; }
        public bool AccountBook { get; set; }
        public bool Class { get; set; }
        public bool Overview => Announcement || ExamSchedule || GradeBook || AccountBook || Class;
    }
}
