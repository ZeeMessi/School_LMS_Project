using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    // Reads the logged-in student's id via User.GetStudentId()!.Value, so
    // this must never be reachable by a Teacher/Admin account - one
    // landing here (a stray link, a typo, deliberate probing) would
    // otherwise hit an unhandled null-reference crash instead of being
    // turned away cleanly.
    [Authorize(Roles = "Student")]
    public class OverviewModel : PageModel
    {
        private readonly AppDbContext _db;

        public OverviewModel(AppDbContext db)
        {
            _db = db;
        }

        // =====================================================
        // STUDENT INFORMATION
        // =====================================================
        //
        // There's no login yet (see Pages/Login.cshtml.cs), so this always
        // shows the first student in the database. Once authentication is
        // wired up, this becomes "the logged-in student" instead.

        public string StudentName { get; set; } = "";

        public string StudentClass { get; set; } = "";

        public string StudentSection { get; set; } = "";

        public string StudentInitials => string.Concat(
            StudentName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => char.ToUpperInvariant(word[0])));


        // =====================================================
        // ATTENDANCE
        // =====================================================

        public double AttendancePercentage { get; set; }

        public int PresentDays { get; set; }

        public int AbsentDays { get; set; }


        // =====================================================
        // OVERALL GRADE
        // =====================================================
        //
        // Null when the student has no recorded exam results yet, so the
        // Grade card can show an empty state instead of a fake "A".

        public string? OverallGrade { get; set; }


        // =====================================================
        // OVERALL PROGRESS
        // =====================================================
        //
        // Computed the same way as OverallGrade (average exam percentage)
        // as a stand-in until Pages/Progress.cshtml gets its own proper
        // term-over-term tracking data.

        public double? OverallProgress { get; set; }

        public string ProgressStatus { get; set; } = "";


        // =====================================================
        // ACCOUNT
        // =====================================================

        public decimal OutstandingDues { get; set; }


        // =====================================================
        // UPCOMING EXAM
        // =====================================================
        //
        // Null when there's nothing scheduled, so the card can show an
        // empty state instead of a fake exam.

        public string? UpcomingExamName { get; set; }

        public string? UpcomingExamSubject { get; set; }

        public string? UpcomingExamDate { get; set; }

        public string? UpcomingExamMonth { get; set; }

        public string? UpcomingExamTime { get; set; }


        public async Task OnGetAsync()
        {
            var studentId = User.GetStudentId()!.Value;

            var student = await _db.Students
                .Include(s => s.ClassRoom)
                .FirstAsync(s => s.Id == studentId);

            StudentName = student.FullName;
            StudentClass = student.ClassRoom.ClassName;
            StudentSection = student.ClassRoom.SectionName;

            await LoadAttendanceAsync(student.Id);
            await LoadGradeAndProgressAsync(student.Id);
            await LoadOutstandingDuesAsync(student.Id);
            await LoadUpcomingExamAsync(student.ClassRoomId);
        }

        private async Task LoadAttendanceAsync(int studentId)
        {
            var records = await _db.AttendanceRecords
                .Where(a => a.StudentId == studentId)
                .Where(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Absent)
                .ToListAsync();

            PresentDays = records.Count(a => a.Status == AttendanceStatus.Present);
            AbsentDays = records.Count(a => a.Status == AttendanceStatus.Absent);

            var recordedDays = PresentDays + AbsentDays;
            AttendancePercentage = recordedDays == 0
                ? 0
                : Math.Round(100.0 * PresentDays / recordedDays, 2);
        }

        private async Task LoadGradeAndProgressAsync(int studentId)
        {
            var results = await _db.ExamResults
                .Where(r => r.StudentId == studentId)
                .ToListAsync();

            if (results.Count == 0)
            {
                return; // leave OverallGrade / OverallProgress null - empty state
            }

            var averagePercentage = results.Average(r => 100.0 * r.ObtainedMarks / r.TotalMarks);

            OverallGrade = GradeScale.LetterFor(averagePercentage);
            OverallProgress = Math.Round(averagePercentage, 0);
            ProgressStatus = averagePercentage switch
            {
                >= 90 => "Excellent progress",
                >= 75 => "Good progress",
                >= 50 => "Needs improvement",
                _ => "At risk"
            };
        }

        private async Task LoadOutstandingDuesAsync(int studentId)
        {
            var unpaidInvoices = await _db.FeeInvoices
                .Where(f => f.StudentId == studentId && f.Status != "Paid")
                .ToListAsync();

            OutstandingDues = unpaidInvoices.Sum(f => f.TotalPayable);
        }

        private async Task LoadUpcomingExamAsync(int classRoomId)
        {
            var today = DateTime.Today;

            var nextExam = await _db.Exams
                .Include(e => e.ExamType)
                .Where(e => e.ClassRoomId == classRoomId && e.Date >= today)
                .OrderBy(e => e.Date)
                .FirstOrDefaultAsync();

            if (nextExam is null)
            {
                return; // leave Upcoming* null - empty state
            }

            UpcomingExamName = $"{nextExam.ExamType.Name} Examination";
            UpcomingExamSubject = nextExam.Subject;
            UpcomingExamDate = nextExam.Date.Day.ToString();
            UpcomingExamMonth = nextExam.Date.ToString("MMM").ToUpperInvariant();
            UpcomingExamTime = nextExam.StartTime;
        }
    }
}
