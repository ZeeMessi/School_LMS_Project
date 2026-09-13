using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class AttendanceModel : PageModel
{
    private readonly AppDbContext _db;

    public AttendanceModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int ClassRoomId { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    public string ClassLabel { get; set; } = "";

    [BindProperty]
    public List<StudentAttendanceRow> Students { get; set; } = new();

    public bool Saved { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var forbidResult = await EnsureTeacherOwnsClassAsync();
        if (forbidResult is not null)
        {
            return forbidResult;
        }

        await LoadAsync();
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var forbidResult = await EnsureTeacherOwnsClassAsync();
        if (forbidResult is not null)
        {
            return forbidResult;
        }

        var studentIds = Students.Select(s => s.StudentId).ToList();

        var existingRecords = await _db.AttendanceRecords
            .Where(a => studentIds.Contains(a.StudentId) && a.Date == Date)
            .ToDictionaryAsync(a => a.StudentId);

        foreach (var row in Students)
        {
            if (existingRecords.TryGetValue(row.StudentId, out var record))
            {
                record.Status = row.Status;
            }
            else
            {
                _db.AttendanceRecords.Add(new AttendanceRecord
                {
                    StudentId = row.StudentId,
                    Date = Date,
                    Status = row.Status
                });
            }
        }

        await _db.SaveChangesAsync();

        Saved = true;
        await LoadAsync();
        return Page();
    }

    // Confirms the logged-in teacher is actually assigned to this class,
    // rather than trusting the classRoomId query parameter outright - a
    // teacher shouldn't be able to open another class's attendance just by
    // editing the URL.
    private async Task<IActionResult?> EnsureTeacherOwnsClassAsync()
    {
        var teacherId = User.GetTeacherId()!.Value;

        var owns = await _db.ClassSubjects
            .AnyAsync(cs => cs.TeacherId == teacherId && cs.ClassRoomId == ClassRoomId);

        return owns ? null : Forbid();
    }

    private async Task LoadAsync()
    {
        var classRoom = await _db.ClassRooms.FirstAsync(c => c.Id == ClassRoomId);
        ClassLabel = $"{classRoom.ClassName} — {classRoom.SectionName}";

        var students = await _db.Students
            .Where(s => s.ClassRoomId == ClassRoomId)
            .OrderBy(s => s.RollNumber)
            .ToListAsync();

        var existingRecords = await _db.AttendanceRecords
            .Where(a => a.Date == Date && students.Select(s => s.Id).Contains(a.StudentId))
            .ToDictionaryAsync(a => a.StudentId);

        Students = students
            .Select(s => new StudentAttendanceRow
            {
                StudentId = s.Id,
                FullName = s.FullName,
                RollNumber = s.RollNumber,
                Status = existingRecords.TryGetValue(s.Id, out var record)
                    ? record.Status
                    : AttendanceStatus.Present
            })
            .ToList();
    }
}

public class StudentAttendanceRow
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = "";

    public string RollNumber { get; set; } = "";

    public AttendanceStatus Status { get; set; }
}
