using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Services;

// Feeds the notification bell in Pages/Shared/_Layout.cshtml's header: a
// combined, most-recent-first activity feed across every category a
// student cares about. This is a feed, not a per-item read/unread
// inbox - the bell's own "something's new" dot instead reuses
// NavTrackingService.NavFlags.Overview, the same aggregate signal the
// sidebar's Overview link shows.
public static class NotificationService
{
    public record NotificationItem(string Icon, string Text, DateTime Timestamp, string Url);

    public static async Task<List<NotificationItem>> GetRecentAsync(
        AppDbContext db, int studentId, int classRoomId, int take = 12)
    {
        var classSubjects = await db.ClassSubjects
            .Where(cs => cs.ClassRoomId == classRoomId)
            .ToListAsync();
        var classSubjectIds = classSubjects.Select(cs => cs.Id).ToList();

        var items = new List<NotificationItem>();

        var materials = await db.CourseMaterials
            .Where(m => classSubjectIds.Contains(m.ClassSubjectId))
            .OrderByDescending(m => m.PostedAt)
            .Take(take)
            .ToListAsync();
        foreach (var m in materials)
        {
            var subjectName = classSubjects.First(cs => cs.Id == m.ClassSubjectId).Name;
            var (icon, section) = m.Type switch
            {
                MaterialType.Handout => ("📘", "handouts"),
                MaterialType.Quiz => ("📝", "quizzes"),
                _ => ("📄", "assignments")
            };
            items.Add(new NotificationItem(
                icon,
                $"{subjectName}: {m.Type} — {m.Title}",
                m.PostedAt,
                $"/SubjectContent?classSubjectId={m.ClassSubjectId}&section={section}"));
        }

        var remarks = await db.TeacherRemarks
            .Where(r => r.StudentId == studentId && classSubjectIds.Contains(r.ClassSubjectId))
            .OrderByDescending(r => r.CreatedAt)
            .Take(take)
            .ToListAsync();
        foreach (var r in remarks)
        {
            var subjectName = classSubjects.First(cs => cs.Id == r.ClassSubjectId).Name;
            items.Add(new NotificationItem(
                "🗒️",
                $"{subjectName}: new teacher remark",
                r.CreatedAt,
                $"/SubjectContent?classSubjectId={r.ClassSubjectId}&section=remarks"));
        }

        var announcements = await db.Announcements
            .Where(a => a.TargetClassRoomId == null || a.TargetClassRoomId == classRoomId)
            .OrderByDescending(a => a.Date)
            .Take(take)
            .ToListAsync();
        foreach (var a in announcements)
        {
            items.Add(new NotificationItem("🔔", $"Announcement: {a.Title}", a.Date, "/Announcement"));
        }

        var exams = await db.Exams
            .Where(e => e.ClassRoomId == classRoomId)
            .OrderByDescending(e => e.CreatedAt)
            .Take(take)
            .ToListAsync();
        foreach (var e in exams)
        {
            items.Add(new NotificationItem("🗓️", $"Exam scheduled: {e.Subject}", e.CreatedAt, "/ExamSchedule"));
        }

        var invoices = await db.FeeInvoices
            .Where(f => f.StudentId == studentId)
            .OrderByDescending(f => f.IssueDate)
            .Take(take)
            .ToListAsync();
        foreach (var f in invoices)
        {
            items.Add(new NotificationItem(
                "₨",
                $"Fee challan issued for {f.BillingPeriod}",
                f.IssueDate.ToDateTime(TimeOnly.MinValue),
                "/AccountBook"));
        }

        var results = await db.ExamResults
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId)
            .OrderByDescending(r => r.RecordedAt)
            .Take(take)
            .ToListAsync();
        foreach (var r in results)
        {
            items.Add(new NotificationItem(
                "🏆",
                $"Grade posted for {r.Exam.Subject}: {r.Grade}",
                r.RecordedAt,
                "/Grade"));
        }

        return items.OrderByDescending(i => i.Timestamp).Take(take).ToList();
    }
}
