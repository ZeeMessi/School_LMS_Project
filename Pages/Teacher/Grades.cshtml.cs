using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;
// SchoolLMS.Pages (the enclosing namespace) already defines a ClassSubject
// view-model record (in class.cshtml.cs) - that would otherwise shadow the
// real entity type here, since enclosing-namespace lookup wins over `using`.
using ClassSubjectEntity = SchoolLMS.Data.Entities.ClassSubject;

namespace SchoolLMS.Pages.Teacher;

[Authorize(Roles = "Teacher")]
public class GradesModel : PageModel
{
    private readonly AppDbContext _db;

    public GradesModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public int ClassSubjectId { get; set; }

    [BindProperty(SupportsGet = true)]
    public string ExamTypeCode { get; set; } = "";

    public string ClassLabel { get; set; } = "";

    public string SubjectName { get; set; } = "";

    public List<ExamTypeOption> ExamTypes { get; set; } = new();

    [BindProperty]
    public int TotalMarks { get; set; } = 100;

    [BindProperty]
    public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [BindProperty]
    public List<StudentGradeRow> Students { get; set; } = new();

    public bool Saved { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var classSubject = await EnsureTeacherOwnsSubjectAsync();
        if (classSubject is null)
        {
            return Forbid();
        }

        await LoadAsync(classSubject);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var classSubject = await EnsureTeacherOwnsSubjectAsync();
        if (classSubject is null)
        {
            return Forbid();
        }

        var examType = await _db.ExamTypes.FirstAsync(t => t.Code == ExamTypeCode);

        var exam = await _db.Exams.Include(e => e.Results).FirstOrDefaultAsync(e =>
            e.ClassRoomId == classSubject.ClassRoomId &&
            e.Subject == classSubject.Name &&
            e.ExamTypeId == examType.Id);

        if (exam is null)
        {
            exam = new Exam
            {
                ClassRoomId = classSubject.ClassRoomId,
                Subject = classSubject.Name,
                ExamTypeId = examType.Id,
                StartTime = "",
                EndTime = "",
                Status = "Completed",
                CreatedAt = DateTime.UtcNow
            };
            _db.Exams.Add(exam);
        }

        exam.Date = ExamDate.ToDateTime(TimeOnly.MinValue);

        var existingResults = await _db.ExamResults
            .Where(r => r.ExamId == exam.Id)
            .ToDictionaryAsync(r => r.StudentId);

        // exam.Id is 0 for a brand-new exam until SaveChanges assigns it,
        // so new exams simply have no existing results to look up yet -
        // every row below becomes an insert in that case.
        foreach (var row in Students)
        {
            var percentage = TotalMarks == 0 ? 0 : 100.0 * row.ObtainedMarks / TotalMarks;
            var grade = GradeScale.LetterFor(percentage);

            if (existingResults.TryGetValue(row.StudentId, out var result))
            {
                result.TotalMarks = TotalMarks;
                result.ObtainedMarks = row.ObtainedMarks;
                result.Grade = grade;
                result.Remarks = row.Remarks;
                result.RecordedAt = DateTime.UtcNow;
            }
            else
            {
                exam.Results.Add(new ExamResult
                {
                    StudentId = row.StudentId,
                    TotalMarks = TotalMarks,
                    ObtainedMarks = row.ObtainedMarks,
                    Grade = grade,
                    Remarks = row.Remarks,
                    RecordedAt = DateTime.UtcNow
                });
            }
        }

        await _db.SaveChangesAsync();

        Saved = true;
        await LoadAsync(classSubject);
        return Page();
    }

    // Confirms the logged-in teacher is actually assigned to this
    // ClassSubject, rather than trusting the classSubjectId query
    // parameter outright.
    private async Task<ClassSubjectEntity?> EnsureTeacherOwnsSubjectAsync()
    {
        var teacherId = User.GetTeacherId()!.Value;

        return await _db.ClassSubjects
            .Include(cs => cs.ClassRoom)
            .FirstOrDefaultAsync(cs => cs.Id == ClassSubjectId && cs.TeacherId == teacherId);
    }

    private async Task LoadAsync(ClassSubjectEntity classSubject)
    {
        ClassLabel = $"{classSubject.ClassRoom.ClassName} — {classSubject.ClassRoom.SectionName}";
        SubjectName = classSubject.Name;

        var examTypesInDb = await _db.ExamTypes.OrderBy(t => t.Id).ToListAsync();

        if (string.IsNullOrWhiteSpace(ExamTypeCode) || examTypesInDb.All(t => t.Code != ExamTypeCode))
        {
            // Default to an exam type that already has an exam recorded
            // for this exact subject/class, so opening the page doesn't
            // land on an empty exam type when a populated one exists.
            var codesWithExam = await _db.Exams
                .Where(e => e.ClassRoomId == classSubject.ClassRoomId && e.Subject == classSubject.Name)
                .Select(e => e.ExamType.Code)
                .Distinct()
                .ToListAsync();

            ExamTypeCode = examTypesInDb.FirstOrDefault(t => codesWithExam.Contains(t.Code))?.Code
                ?? examTypesInDb.FirstOrDefault()?.Code
                ?? "";
        }

        ExamTypes = examTypesInDb
            .Select(t => new ExamTypeOption(t.Code, t.Name, t.Code == ExamTypeCode))
            .ToList();

        var students = await _db.Students
            .Where(s => s.ClassRoomId == classSubject.ClassRoomId)
            .OrderBy(s => s.RollNumber)
            .ToListAsync();

        var exam = await _db.Exams
            .Include(e => e.Results)
            .FirstOrDefaultAsync(e =>
                e.ClassRoomId == classSubject.ClassRoomId &&
                e.Subject == classSubject.Name &&
                e.ExamType.Code == ExamTypeCode);

        if (exam is not null)
        {
            ExamDate = DateOnly.FromDateTime(exam.Date);
            TotalMarks = exam.Results.FirstOrDefault()?.TotalMarks ?? 100;
        }

        var resultsByStudent = exam?.Results.ToDictionary(r => r.StudentId) ?? new();

        Students = students
            .Select(s => new StudentGradeRow
            {
                StudentId = s.Id,
                FullName = s.FullName,
                RollNumber = s.RollNumber,
                ObtainedMarks = resultsByStudent.TryGetValue(s.Id, out var result) ? result.ObtainedMarks : 0,
                Remarks = resultsByStudent.TryGetValue(s.Id, out var r2) ? r2.Remarks ?? "" : ""
            })
            .ToList();
    }
}

public record ExamTypeOption(string Code, string Name, bool IsActive);

public class StudentGradeRow
{
    public int StudentId { get; set; }

    public string FullName { get; set; } = "";

    public string RollNumber { get; set; } = "";

    public int ObtainedMarks { get; set; }

    public string Remarks { get; set; } = "";
}
