using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Services;

// Powers the "new" badge on class.cshtml's subject-option boxes and on
// SubjectContent.cshtml's tabs: for each (ClassSubject, ContentSection)
// pair, compares the newest item actually posted against the student's
// StudentSectionView row (their last visit) so the badge is a real
// signal rather than always being on.
public static class ContentTrackingService
{
    public static async Task<Dictionary<(int ClassSubjectId, ContentSection Section), bool>> GetNewFlagsAsync(
        AppDbContext db,
        int studentId,
        int classRoomId,
        List<ClassSubject> classSubjects)
    {
        var classSubjectIds = classSubjects.Select(cs => cs.Id).ToList();
        var teacherIds = classSubjects
            .Where(cs => cs.TeacherId.HasValue)
            .Select(cs => cs.TeacherId!.Value)
            .Distinct()
            .ToList();

        var latestMaterials = await db.CourseMaterials
            .Where(m => classSubjectIds.Contains(m.ClassSubjectId))
            .GroupBy(m => new { m.ClassSubjectId, m.Type })
            .Select(g => new { g.Key.ClassSubjectId, g.Key.Type, Latest = g.Max(m => m.PostedAt) })
            .ToListAsync();

        var latestRemarks = await db.TeacherRemarks
            .Where(r => classSubjectIds.Contains(r.ClassSubjectId) && r.StudentId == studentId)
            .GroupBy(r => r.ClassSubjectId)
            .Select(g => new { ClassSubjectId = g.Key, Latest = g.Max(r => r.CreatedAt) })
            .ToListAsync();

        // Announcements aren't tied to a ClassSubject directly (see
        // Announcement.cs) - a subject's "Announcement" box means
        // "posted by the teacher who teaches this subject", so this is
        // grouped by TeacherId instead and matched up below.
        var latestAnnouncementsByTeacher = await db.Announcements
            .Where(a => a.TeacherId != null
                && teacherIds.Contains(a.TeacherId!.Value)
                && (a.TargetClassRoomId == null || a.TargetClassRoomId == classRoomId))
            .GroupBy(a => a.TeacherId!.Value)
            .Select(g => new { TeacherId = g.Key, Latest = g.Max(a => a.Date) })
            .ToListAsync();

        var views = await db.StudentSectionViews
            .Where(v => v.StudentId == studentId && classSubjectIds.Contains(v.ClassSubjectId))
            .ToListAsync();
        var viewLookup = views.ToDictionary(v => (v.ClassSubjectId, v.Section), v => v.LastViewedAt);

        bool HasNew(int classSubjectId, ContentSection section, DateTime? latest)
        {
            if (latest is null)
            {
                return false;
            }

            return !viewLookup.TryGetValue((classSubjectId, section), out var lastViewed) || latest > lastViewed;
        }

        var result = new Dictionary<(int, ContentSection), bool>();

        foreach (var cs in classSubjects)
        {
            var assignmentLatest = latestMaterials
                .FirstOrDefault(m => m.ClassSubjectId == cs.Id && m.Type == MaterialType.Assignment)?.Latest;
            var handoutLatest = latestMaterials
                .FirstOrDefault(m => m.ClassSubjectId == cs.Id && m.Type == MaterialType.Handout)?.Latest;
            var quizLatest = latestMaterials
                .FirstOrDefault(m => m.ClassSubjectId == cs.Id && m.Type == MaterialType.Quiz)?.Latest;
            var remarkLatest = latestRemarks
                .FirstOrDefault(r => r.ClassSubjectId == cs.Id)?.Latest;
            var announcementLatest = cs.TeacherId.HasValue
                ? latestAnnouncementsByTeacher.FirstOrDefault(a => a.TeacherId == cs.TeacherId.Value)?.Latest
                : null;

            result[(cs.Id, ContentSection.Assignment)] = HasNew(cs.Id, ContentSection.Assignment, assignmentLatest);
            result[(cs.Id, ContentSection.Handout)] = HasNew(cs.Id, ContentSection.Handout, handoutLatest);
            result[(cs.Id, ContentSection.Quiz)] = HasNew(cs.Id, ContentSection.Quiz, quizLatest);
            result[(cs.Id, ContentSection.Remark)] = HasNew(cs.Id, ContentSection.Remark, remarkLatest);
            result[(cs.Id, ContentSection.Announcement)] = HasNew(cs.Id, ContentSection.Announcement, announcementLatest);
        }

        return result;
    }

    // Called when a student opens one of the five boxes/tabs, so the
    // "new" badge clears for next time.
    public static async Task MarkViewedAsync(AppDbContext db, int studentId, int classSubjectId, ContentSection section)
    {
        var existing = await db.StudentSectionViews.FirstOrDefaultAsync(v =>
            v.StudentId == studentId && v.ClassSubjectId == classSubjectId && v.Section == section);

        if (existing is null)
        {
            db.StudentSectionViews.Add(new StudentSectionView
            {
                StudentId = studentId,
                ClassSubjectId = classSubjectId,
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
}
