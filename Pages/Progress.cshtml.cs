using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages;

// See Overview.cshtml.cs - reads User.GetStudentId()!.Value, so must stay
// restricted to the Student role.
//
// Every number on this page is computed from real ExamResult/
// AttendanceRecord rows - there's no separate "term" or "assessment type"
// concept in the schema, so the original hardcoded filters for those were
// dropped in favor of the one filter that maps to something real: Subject.
// With only two real exam sittings on record (Monthly Test and Quarterly -
// see Data/DbSeeder.cs), the trend/breakdown charts are necessarily
// sparser than the old fabricated 8-month history, but every point on them
// is real.
[Authorize(Roles = "Student")]
public class ProgressModel : PageModel
{
    private readonly AppDbContext _db;

    public ProgressModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty(SupportsGet = true)]
    public string Subject { get; set; } = "All";

    public string ClassLabel { get; set; } = "";

    public string AcademicYear { get; set; } = "";

    public List<string> AvailableSubjects { get; set; } = new();

    public double? OverallPercentage { get; set; }

    public string? OverallGrade { get; set; }

    public string ProgressStatus { get; set; } = "";

    public double? ImprovementPercentage { get; set; }

    public int SubjectsImprovingCount { get; set; }

    public int SubjectsComparableCount { get; set; }

    // Shared by both the trend line and the breakdown doughnut - they're
    // two views of the same real sittings, not two separate data sources.
    public List<SittingPoint> Sittings { get; set; } = new();

    public List<SubjectProgressRow> SubjectProgress { get; set; } = new();

    public List<SubjectHighlight> Strengths { get; set; } = new();

    public List<SubjectHighlight> NeedsAttention { get; set; } = new();

    public List<MonthlyPoint> AttendanceVsPerformance { get; set; } = new();

    public List<TimelineEntry> Timeline { get; set; } = new();

    public async Task OnGetAsync()
    {
        var studentId = User.GetStudentId()!.Value;

        var student = await _db.Students.Include(s => s.ClassRoom).FirstAsync(s => s.Id == studentId);
        ClassLabel = $"{student.ClassRoom.ClassName} — {student.ClassRoom.SectionName}";
        AcademicYear = student.ClassRoom.AcademicYear;

        var results = await _db.ExamResults
            .Include(r => r.Exam).ThenInclude(e => e.ExamType)
            .Where(r => r.StudentId == studentId)
            .ToListAsync();

        AvailableSubjects = results.Select(r => r.Exam.Subject).Distinct().OrderBy(s => s).ToList();

        if (Subject != "All" && !AvailableSubjects.Contains(Subject))
        {
            Subject = "All";
        }

        BuildSittings(results);
        BuildSubjectProgress(results);
        BuildHighlights();
        await BuildAttendanceVsPerformanceAsync(studentId, results);
        BuildTimeline();
    }

    private void BuildSittings(List<ExamResult> results)
    {
        var bySittingType = results
            .GroupBy(r => new { r.Exam.ExamTypeId, r.Exam.ExamType.Name })
            .Select(g => new
            {
                g.Key.Name,
                Date = g.Max(r => r.Exam.Date),
                SubjectPercentages = g
                    .GroupBy(r => r.Exam.Subject)
                    .ToDictionary(sg => sg.Key, sg => Math.Round(sg.Average(r => 100.0 * r.ObtainedMarks / r.TotalMarks), 1))
            })
            .OrderBy(s => s.Date)
            .ToList();

        foreach (var sitting in bySittingType)
        {
            double? percentage = Subject == "All"
                ? Math.Round(sitting.SubjectPercentages.Values.Average(), 1)
                : sitting.SubjectPercentages.TryGetValue(Subject, out var subjectPct) ? subjectPct : null;

            if (percentage is not null)
            {
                Sittings.Add(new SittingPoint(sitting.Name, sitting.Date, percentage.Value, sitting.SubjectPercentages.Count));
            }
        }

        if (Sittings.Count == 0)
        {
            return;
        }

        OverallPercentage = Sittings[^1].Percentage;
        OverallGrade = GradeScale.LetterFor(OverallPercentage.Value);
        ProgressStatus = OverallPercentage switch
        {
            >= 90 => "Excellent progress",
            >= 75 => "Good progress",
            >= 50 => "Needs improvement",
            _ => "At risk"
        };

        if (Sittings.Count >= 2)
        {
            ImprovementPercentage = Math.Round(Sittings[^1].Percentage - Sittings[0].Percentage, 1);
        }
    }

    private void BuildSubjectProgress(List<ExamResult> results)
    {
        var bySubject = results
            .GroupBy(r => r.Exam.Subject)
            .ToDictionary(
                g => g.Key,
                g => g.Select(r => (r.Exam.Date, Percentage: 100.0 * r.ObtainedMarks / r.TotalMarks))
                      .OrderBy(x => x.Date)
                      .ToList());

        foreach (var (subject, points) in bySubject.OrderBy(kvp => kvp.Key))
        {
            var latest = points[^1];
            double? delta = points.Count >= 2 ? Math.Round(latest.Percentage - points[^2].Percentage, 1) : null;

            SubjectProgress.Add(new SubjectProgressRow(subject, Math.Round(latest.Percentage, 1), delta));

            if (delta is not null)
            {
                SubjectsComparableCount++;
                if (delta > 0)
                {
                    SubjectsImprovingCount++;
                }
            }
        }

        foreach (var row in SubjectProgress)
        {
            if (row.LatestPercentage >= 85 && row.Delta is not < 0)
            {
                Strengths.Add(new SubjectHighlight(row.Subject, row.LatestPercentage, "Excellent"));
            }
            else if (row.LatestPercentage < 80 || row.Delta < 0)
            {
                var note = row.Delta < 0 ? "Declining - worth a closer look" : "Below target";
                NeedsAttention.Add(new SubjectHighlight(row.Subject, row.LatestPercentage, note));
            }
        }
    }

    private void BuildHighlights()
    {
        // Highest first for strengths, lowest first for attention - reads
        // naturally either way, and both lists were already populated in
        // BuildSubjectProgress.
        Strengths = Strengths.OrderByDescending(s => s.Percentage).ToList();
        NeedsAttention = NeedsAttention.OrderBy(s => s.Percentage).ToList();
    }

    private async Task BuildAttendanceVsPerformanceAsync(int studentId, List<ExamResult> results)
    {
        var attendance = await _db.AttendanceRecords
            .Where(a => a.StudentId == studentId)
            .Where(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Absent)
            .ToListAsync();

        var attendanceByMonth = attendance
            .GroupBy(a => new DateTime(a.Date.Year, a.Date.Month, 1))
            .ToDictionary(g => g.Key, g =>
            {
                var present = g.Count(a => a.Status == AttendanceStatus.Present);
                return Math.Round(100.0 * present / g.Count(), 1);
            });

        var academicByMonth = results
            .GroupBy(r => new DateTime(r.Exam.Date.Year, r.Exam.Date.Month, 1))
            .ToDictionary(g => g.Key, g => Math.Round(g.Average(r => 100.0 * r.ObtainedMarks / r.TotalMarks), 1));

        var months = attendanceByMonth.Keys.Union(academicByMonth.Keys).OrderBy(m => m);

        foreach (var month in months)
        {
            AttendanceVsPerformance.Add(new MonthlyPoint(
                month.ToString("MMM yyyy"),
                attendanceByMonth.TryGetValue(month, out var att) ? att : null,
                academicByMonth.TryGetValue(month, out var acad) ? acad : null));
        }
    }

    private void BuildTimeline()
    {
        foreach (var sitting in Sittings.OrderBy(s => s.Date))
        {
            Timeline.Add(new TimelineEntry(
                sitting.Date,
                $"{sitting.ExamTypeName} Completed",
                $"Overall {sitting.Percentage}% across {sitting.SubjectCount} subject(s)."));
        }
    }
}

public record SittingPoint(string ExamTypeName, DateTime Date, double Percentage, int SubjectCount);

public record SubjectProgressRow(string Subject, double LatestPercentage, double? Delta);

public record SubjectHighlight(string Subject, double Percentage, string Note);

public record MonthlyPoint(string MonthLabel, double? AttendancePercentage, double? AcademicPercentage);

public record TimelineEntry(DateTime Date, string Title, string Description);
