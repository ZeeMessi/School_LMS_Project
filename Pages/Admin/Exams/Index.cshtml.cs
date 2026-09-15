using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
// SchoolLMS.Pages (an enclosing namespace) already defines a ClassSubject
// view-model (in class.cshtml.cs) that would otherwise shadow the real
// entity type here, since enclosing-namespace lookup wins over `using`.
using ClassSubjectEntity = SchoolLMS.Data.Entities.ClassSubject;

namespace SchoolLMS.Pages.Admin.Exams;

// Lets Admin publish/update an exam timetable (subject, date, time,
// room, invigilator) for one exam type (Quarterly/Bi-Annual/Annual/
// Monthly - see DbSeeder.SeedExamTypes) and one class. Every add also
// upserts a companion Announcement so students see it on their
// Announcement page and get a "new" badge there and on Exam Schedule -
// see UpsertScheduleAnnouncementAsync.
[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int ExamTypeId { get; set; }

    [BindProperty(SupportsGet = true)]
    public int ClassRoomId { get; set; }

    [BindProperty]
    public int ClassSubjectId { get; set; }

    [BindProperty]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [BindProperty]
    public string StartTime { get; set; } = "";

    [BindProperty]
    public string EndTime { get; set; } = "";

    [BindProperty]
    public string Room { get; set; } = "";

    [BindProperty]
    public string Invigilator { get; set; } = "";

    public List<ExamType> ExamTypes { get; set; } = new();

    public List<ClassRoom> ClassRooms { get; set; } = new();

    public List<ClassSubjectEntity> SubjectOptions { get; set; } = new();

    public List<Exam> ScheduledExams { get; set; } = new();

    public string? ErrorMessage { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        await LoadAsync();
    }

    public async Task<IActionResult> OnPostAddAsync()
    {
        await LoadAsync();

        var subject = SubjectOptions.FirstOrDefault(cs => cs.Id == ClassSubjectId);
        if (subject is null)
        {
            ErrorMessage = "Choose a subject actually taught to this class.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(StartTime) || string.IsNullOrWhiteSpace(EndTime))
        {
            ErrorMessage = "Start time and end time are both required.";
            return Page();
        }

        var examType = await _db.ExamTypes.FirstAsync(t => t.Id == ExamTypeId);

        // Matches Teacher/Grades.cshtml.cs's own lookup (ClassRoom + Subject
        // + ExamType) so a teacher entering results later for the same
        // subject/type updates this exact row instead of creating a
        // second, disconnected one.
        var exam = await _db.Exams.FirstOrDefaultAsync(e =>
            e.ClassRoomId == ClassRoomId && e.Subject == subject.Name && e.ExamTypeId == ExamTypeId);

        if (exam is null)
        {
            exam = new Exam
            {
                ClassRoomId = ClassRoomId,
                ExamTypeId = ExamTypeId,
                Subject = subject.Name,
                CreatedAt = DateTime.UtcNow
            };
            _db.Exams.Add(exam);
        }

        exam.Date = Date.ToDateTime(TimeOnly.MinValue);
        exam.StartTime = StartTime;
        exam.EndTime = EndTime;
        exam.Room = string.IsNullOrWhiteSpace(Room) ? null : Room.Trim();
        exam.Invigilator = string.IsNullOrWhiteSpace(Invigilator) ? null : Invigilator.Trim();
        exam.Status = exam.Date.Date >= DateTime.Today ? "Upcoming" : "Completed";

        await _db.SaveChangesAsync();

        await UpsertScheduleAnnouncementAsync(examType);

        StatusMessage = $"{subject.Name} added to the {examType.Name} schedule.";
        return RedirectToPage(new { ExamTypeId, ClassRoomId });
    }

    public async Task<IActionResult> OnPostDeleteAsync(int id)
    {
        var exam = await _db.Exams.FirstOrDefaultAsync(e => e.Id == id && e.ClassRoomId == ClassRoomId);
        if (exam is not null)
        {
            _db.Exams.Remove(exam);
            await _db.SaveChangesAsync();
            StatusMessage = "Removed from the schedule.";
        }

        return RedirectToPage(new { ExamTypeId, ClassRoomId });
    }

    // One announcement per (exam type, class) combination, kept up to
    // date rather than reposted every time a subject is added to the
    // same schedule - see Announcement.RelatedExamTypeId.
    private async Task UpsertScheduleAnnouncementAsync(ExamType examType)
    {
        var subjectCount = await _db.Exams.CountAsync(e => e.ClassRoomId == ClassRoomId && e.ExamTypeId == ExamTypeId);

        var announcement = await _db.Announcements.FirstOrDefaultAsync(a =>
            a.RelatedExamTypeId == ExamTypeId && a.TargetClassRoomId == ClassRoomId);

        var description = $"The {examType.Name} exam schedule has been published/updated - " +
            $"{subjectCount} subject{(subjectCount == 1 ? "" : "s")} scheduled so far. " +
            "Check Exam Schedule for full details.";

        if (announcement is null)
        {
            _db.Announcements.Add(new Announcement
            {
                Title = $"{examType.Name} Examination Schedule",
                Category = "Exam",
                Date = DateTime.UtcNow,
                ShortDescription = description,
                FullDescription = description,
                IssuedBy = "School Administration",
                IsImportant = true,
                TargetClassRoomId = ClassRoomId,
                RelatedExamTypeId = ExamTypeId
            });
        }
        else
        {
            // Bumping Date resurfaces it at the top of the Announcement
            // feed and re-triggers the "new" badge for anyone who already
            // saw the earlier version of this schedule.
            announcement.Date = DateTime.UtcNow;
            announcement.ShortDescription = description;
            announcement.FullDescription = description;
        }

        await _db.SaveChangesAsync();
    }

    private async Task LoadAsync()
    {
        ExamTypes = await _db.ExamTypes.OrderBy(t => t.Id).ToListAsync();
        ClassRooms = await _db.ClassRooms.OrderBy(c => c.ClassName).ThenBy(c => c.SectionName).ToListAsync();

        if (ExamTypeId == 0)
        {
            ExamTypeId = ExamTypes.FirstOrDefault()?.Id ?? 0;
        }

        if (ClassRoomId == 0)
        {
            ClassRoomId = ClassRooms.FirstOrDefault()?.Id ?? 0;
        }

        SubjectOptions = await _db.ClassSubjects
            .Where(cs => cs.ClassRoomId == ClassRoomId)
            .OrderBy(cs => cs.Name)
            .ToListAsync();

        ScheduledExams = await _db.Exams
            .Where(e => e.ClassRoomId == ClassRoomId && e.ExamTypeId == ExamTypeId)
            .OrderBy(e => e.Date)
            .ToListAsync();
    }
}
