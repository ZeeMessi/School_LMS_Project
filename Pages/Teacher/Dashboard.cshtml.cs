using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Services;

namespace SchoolLMS.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class DashboardModel : PageModel
{
    private readonly AppDbContext _db;

    public DashboardModel(AppDbContext db)
    {
        _db = db;
    }

    public string TeacherName { get; set; } = "";

    public List<TaughtClassRow> TaughtClasses { get; set; } = new();

    public async Task OnGetAsync()
    {
        var teacherId = User.GetTeacherId()!.Value;

        var teacher = await _db.Teachers.FirstAsync(t => t.Id == teacherId);
        TeacherName = teacher.FullName;

        var assignments = await _db.ClassSubjects
            .Include(cs => cs.ClassRoom)
            .Where(cs => cs.TeacherId == teacherId)
            .ToListAsync();

        // Student counts per class, computed in one query rather than
        // one round-trip per row.
        var classRoomIds = assignments.Select(a => a.ClassRoomId).Distinct().ToList();
        var studentCounts = await _db.Students
            .Where(s => classRoomIds.Contains(s.ClassRoomId))
            .GroupBy(s => s.ClassRoomId)
            .Select(g => new { ClassRoomId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ClassRoomId, x => x.Count);

        TaughtClasses = assignments
            .Select(a => new TaughtClassRow(
                a.Id,
                a.ClassRoomId,
                $"{a.ClassRoom.ClassName} — {a.ClassRoom.SectionName}",
                a.Name,
                studentCounts.GetValueOrDefault(a.ClassRoomId, 0)))
            .ToList();
    }
}

public record TaughtClassRow(int ClassSubjectId, int ClassRoomId, string ClassLabel, string SubjectName, int StudentCount);
