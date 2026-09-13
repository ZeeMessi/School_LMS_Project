using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    public class AttendanceModel : PageModel
    {
        private readonly AppDbContext _db;

        public AttendanceModel(AppDbContext db)
        {
            _db = db;
        }

        public string ClassName { get; set; } = "";

        public string SectionName { get; set; } = "";

        public double OverallAttendancePercentage { get; set; }

        public string CurrentMonthName { get; set; } = "";

        public List<string> WeekDays { get; } = new()
        {
            "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
        };

        public List<AttendanceDay> CalendarDays { get; set; } = new();

        public AttendanceDay? Today { get; set; }

        public int MonthlyPresentCount { get; set; }
        public int MonthlyAbsentCount { get; set; }
        public int MonthlyHolidayCount { get; set; }
        public int MonthlyNotRecordedCount { get; set; }
        public double MonthlyAttendancePercentage { get; set; }
        public int MonthlyRecordedDays { get; set; }

        public async Task OnGetAsync()
        {
            var studentId = User.GetStudentId()!.Value;

            var student = await _db.Students
                .Include(s => s.ClassRoom)
                .FirstAsync(s => s.Id == studentId);

            ClassName = student.ClassRoom.ClassName;
            SectionName = student.ClassRoom.SectionName;

            var allRecords = await _db.AttendanceRecords
                .Where(a => a.StudentId == student.Id)
                .ToListAsync();

            OverallAttendancePercentage = ComputePercentage(allRecords);

            // "This month" is whichever month actually has recorded
            // attendance, since only one month is seeded so far. Once a
            // real school has a full year of data, this would instead be
            // driven by a month picker (the Prev/Next buttons in the view
            // are still inert placeholders, same as before this page was
            // wired to the database).
            var latestMonth = allRecords.Count == 0
                ? DateOnly.FromDateTime(DateTime.Today)
                : allRecords.Max(a => a.Date);

            var monthRecords = allRecords
                .Where(a => a.Date.Year == latestMonth.Year && a.Date.Month == latestMonth.Month)
                .ToList();

            CurrentMonthName = new DateTime(latestMonth.Year, latestMonth.Month, 1).ToString("MMMM");
            BuildCalendar(latestMonth, monthRecords);

            var today = DateOnly.FromDateTime(DateTime.Today);
            Today = CalendarDays.FirstOrDefault(d => d.Date == today && d.IsCurrentMonth);

            MonthlyPresentCount = monthRecords.Count(a => a.Status == AttendanceStatus.Present);
            MonthlyAbsentCount = monthRecords.Count(a => a.Status == AttendanceStatus.Absent);
            MonthlyHolidayCount = monthRecords.Count(a => a.Status == AttendanceStatus.Weekend || a.Status == AttendanceStatus.Holiday);
            MonthlyNotRecordedCount = monthRecords.Count(a => a.Status == AttendanceStatus.NotRecorded);
            MonthlyRecordedDays = MonthlyPresentCount + MonthlyAbsentCount;
            MonthlyAttendancePercentage = ComputePercentage(monthRecords);
        }

        private static double ComputePercentage(List<AttendanceRecord> records)
        {
            var present = records.Count(a => a.Status == AttendanceStatus.Present);
            var absent = records.Count(a => a.Status == AttendanceStatus.Absent);
            var recorded = present + absent;

            return recorded == 0 ? 0 : Math.Round(100.0 * present / recorded, 2);
        }

        private void BuildCalendar(DateOnly month, List<AttendanceRecord> monthRecords)
        {
            // Value type is explicitly AttendanceStatus? here, not
            // AttendanceStatus - otherwise TryGetValue's failure case below
            // (no record for that date) leaves `status` at
            // default(AttendanceStatus), which is Present (the enum's 0
            // value), silently turning "no record" into "marked present".
            var recordsByDate = monthRecords.ToDictionary(a => a.Date, a => (AttendanceStatus?)a.Status);

            var firstOfMonth = new DateOnly(month.Year, month.Month, 1);
            var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
            var startOffset = ((int)firstOfMonth.DayOfWeek + 6) % 7; // Monday-first grid

            var today = DateOnly.FromDateTime(DateTime.Today);

            CalendarDays = new List<AttendanceDay>();

            for (var i = startOffset - 1; i >= 0; i--)
            {
                var date = firstOfMonth.AddDays(-(i + 1));
                CalendarDays.Add(new AttendanceDay(date, false, date == today, null));
            }

            for (var day = 1; day <= daysInMonth; day++)
            {
                var date = new DateOnly(month.Year, month.Month, day);
                recordsByDate.TryGetValue(date, out var status);
                CalendarDays.Add(new AttendanceDay(date, true, date == today, status));
            }

            while (CalendarDays.Count % 7 != 0)
            {
                var nextDate = CalendarDays[^1].Date.AddDays(1);
                CalendarDays.Add(new AttendanceDay(nextDate, false, nextDate == today, null));
            }
        }
    }

    public record AttendanceDay(DateOnly Date, bool IsCurrentMonth, bool IsToday, AttendanceStatus? Status)
    {
        public int DayNumber => Date.Day;

        public string CssClass => Status switch
        {
            AttendanceStatus.Present => "status-present",
            AttendanceStatus.Absent => "status-absent",
            AttendanceStatus.Weekend or AttendanceStatus.Holiday => "status-weekend",
            AttendanceStatus.NotRecorded => "status-not-recorded",
            _ => "outside-month"
        };

        public string? StatusIcon => Status switch
        {
            AttendanceStatus.Present => "✓",
            AttendanceStatus.Absent => "×",
            AttendanceStatus.Weekend or AttendanceStatus.Holiday => "OFF",
            AttendanceStatus.NotRecorded => "—",
            _ => null
        };

        public string? StatusLabel => Status switch
        {
            AttendanceStatus.Present => "Present",
            AttendanceStatus.Absent => "Absent",
            AttendanceStatus.Weekend or AttendanceStatus.Holiday => "Weekend",
            AttendanceStatus.NotRecorded => "Not Recorded",
            _ => null
        };
    }
}
