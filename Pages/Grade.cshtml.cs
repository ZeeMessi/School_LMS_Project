using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    // See Overview.cshtml.cs - reads User.GetStudentId()!.Value, so must
    // stay restricted to the Student role.
    [Authorize(Roles = "Student")]
    public class GradeModel : PageModel
    {
        private readonly AppDbContext _db;

        public GradeModel(AppDbContext db)
        {
            _db = db;
        }

        // Which exam type's results to show - a real navigation (page
        // reload with ?examCode=...), not a client-side data switch, since
        // the exam-type buttons used to switch between four hardcoded JS
        // datasets that could (and did) disagree with the server-rendered
        // table underneath them.
        [BindProperty(SupportsGet = true)]
        public string ExamCode { get; set; } = "";

        public string StudentName { get; set; } = "";

        public string RollNumber { get; set; } = "";

        public string ClassName { get; set; } = "";

        public string SectionName { get; set; } = "";

        public string AcademicYear { get; set; } = "";

        public double OverallPercentage { get; set; }

        public string OverallGrade { get; set; } = "";

        public int TotalObtainedMarks { get; set; }

        public int TotalMaxMarks { get; set; }

        public int SubjectsPassed { get; set; }

        public int SubjectsCount { get; set; }

        public List<SubjectResultRow> SubjectResults { get; set; } = new();

        // The exam-type buttons at the top of the page - whatever exam
        // types this school has actually configured, not a fixed list of
        // four. A school with only Quarterly + Annual just won't have the
        // other rows.
        public List<ExamTypeOption> AvailableExamTypes { get; set; } = new();

        public List<HistoryPoint> PerformanceHistory { get; set; } = new();

        public async Task OnGetAsync()
        {
            var studentId = User.GetStudentId()!.Value;

            var student = await _db.Students
                .Include(s => s.ClassRoom)
                .FirstAsync(s => s.Id == studentId);

            StudentName = student.FullName;
            RollNumber = student.RollNumber;
            ClassName = student.ClassRoom.ClassName;
            SectionName = student.ClassRoom.SectionName;
            AcademicYear = student.ClassRoom.AcademicYear;

            var examTypes = await _db.ExamTypes.OrderBy(t => t.Id).ToListAsync();

            if (string.IsNullOrWhiteSpace(ExamCode) || examTypes.All(t => t.Code != ExamCode))
            {
                // Default to an exam type that actually has recorded
                // results for this student, rather than just whichever
                // exam type happens to be first - a school might configure
                // its exam types in any order.
                var codesWithResults = await _db.ExamResults
                    .Where(r => r.StudentId == student.Id)
                    .Select(r => r.Exam.ExamType.Code)
                    .Distinct()
                    .ToListAsync();

                ExamCode = examTypes.FirstOrDefault(t => codesWithResults.Contains(t.Code))?.Code
                    ?? examTypes.FirstOrDefault()?.Code
                    ?? "";
            }

            AvailableExamTypes = examTypes
                .Select(t => new ExamTypeOption(t.Code, t.Name, t.Code == ExamCode))
                .ToList();

            var results = await _db.ExamResults
                .Include(r => r.Exam)
                .Where(r => r.StudentId == student.Id && r.Exam.ExamType.Code == ExamCode)
                .ToListAsync();

            SubjectResults = results
                .Select(r => new SubjectResultRow(
                    r.Exam.Subject,
                    r.TotalMarks,
                    r.ObtainedMarks,
                    r.Grade,
                    r.Remarks ?? ""))
                .ToList();

            TotalMaxMarks = results.Sum(r => r.TotalMarks);
            TotalObtainedMarks = results.Sum(r => r.ObtainedMarks);
            OverallPercentage = TotalMaxMarks == 0
                ? 0
                : Math.Round(100.0 * TotalObtainedMarks / TotalMaxMarks, 2);
            OverallGrade = TotalMaxMarks == 0 ? "" : GradeScale.LetterFor(OverallPercentage);

            SubjectsCount = results.Count;
            SubjectsPassed = results.Count(r => 100.0 * r.ObtainedMarks / r.TotalMarks >= 50);

            await LoadPerformanceHistoryAsync(student.Id);
        }

        private async Task LoadPerformanceHistoryAsync(int studentId)
        {
            // One point per exam type the student actually has results for,
            // in chronological order of when those exams were sat - rather
            // than the old hardcoded Monthly/Quarterly/Mid-Term/Annual
            // ordering, which didn't correspond to any real dates.
            var allResults = await _db.ExamResults
                .Include(r => r.Exam)
                    .ThenInclude(e => e.ExamType)
                .Where(r => r.StudentId == studentId)
                .ToListAsync();

            PerformanceHistory = allResults
                .GroupBy(r => new { r.Exam.ExamType.Code, r.Exam.ExamType.Name })
                .Select(g => new HistoryPoint(
                    g.Key.Name,
                    Math.Round(g.Average(r => 100.0 * r.ObtainedMarks / r.TotalMarks), 1),
                    g.Min(r => r.Exam.Date)))
                .OrderBy(p => p.EarliestExamDate)
                .ToList();
        }
    }

    public record SubjectResultRow(string Subject, int TotalMarks, int ObtainedMarks, string Grade, string Remarks)
    {
        public double Percentage => TotalMarks == 0 ? 0 : Math.Round(100.0 * ObtainedMarks / TotalMarks, 1);

        public string GradeCssClass => Grade switch
        {
            "A+" => "grade-aplus",
            "A" => "grade-a",
            "B+" => "grade-bplus",
            _ => ""
        };
    }

    public record ExamTypeOption(string Code, string Name, bool IsActive);

    public record HistoryPoint(string ExamTypeName, double AveragePercentage, DateTime EarliestExamDate);
}
