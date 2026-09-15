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
[Authorize(Roles = "Student")]
public class ExamScheduleModel : PageModel
{
    private readonly AppDbContext _db;

    public ExamScheduleModel(AppDbContext db)
    {
        _db = db;
    }

    // ------------------------------------------------------------
    // STUDENT / CLASS INFORMATION
    // ------------------------------------------------------------

    public string ClassName { get; set; } = "";

    public string SectionName { get; set; } = "";

    public string AcademicYear { get; set; } = "";


    // ------------------------------------------------------------
    // CURRENT EXAMINATION TYPE
    // ------------------------------------------------------------

    // Real navigation (?examTypeCode=...), same approach as Grade.cshtml -
    // not a client-side data swap.
    [BindProperty(SupportsGet = true)]
    public string ExamTypeCode { get; set; } = "";


    // ------------------------------------------------------------
    // DISPLAY TOGGLES
    // ------------------------------------------------------------
    //
    // Rather than separate school-configuration flags unbacked by any
    // real data, these just reflect whether the currently-filtered exams
    // actually have that information - so the columns adapt to the data.

    public bool ShowSubjectCode { get; set; }

    public bool ShowRoom { get; set; }

    public bool ShowInvigilator { get; set; }


    // ------------------------------------------------------------
    // EXAM TYPES
    // ------------------------------------------------------------

    public List<ExamType> ExamTypes { get; set; } = new();


    // ------------------------------------------------------------
    // EXAMINATION DATA
    // ------------------------------------------------------------

    public List<ExamItem> Exams { get; set; } = new();

    public List<ExamItem> UpcomingExams { get; set; } = new();


    // ------------------------------------------------------------
    // CALENDAR
    // ------------------------------------------------------------

    public string CurrentMonthName { get; set; } = "";

    public List<string> WeekDays { get; set; } = new();

    public List<CalendarDay> CalendarDays { get; set; } = new();


    // ------------------------------------------------------------
    // SUMMARY
    // ------------------------------------------------------------

    public int TotalExams => Exams.Count;

    public int TotalSubjects => Exams.Select(x => x.Subject).Distinct().Count();

    public int UpcomingExamCount => UpcomingExams.Count;


    // ------------------------------------------------------------
    // EXAM INSTRUCTIONS
    // ------------------------------------------------------------
    //
    // Still a static default - there's no per-school/per-exam-type
    // instructions entity yet, same caveat as Grade.cshtml's general
    // teacher remark.

    public bool ShowInstructions { get; set; } = true;

    public string ExamInstructions { get; set; } =
        "Students should arrive at least 15 minutes before the examination. " +
        "Bring the required stationery and examination materials.";


    public async Task OnGetAsync()
    {
        var studentId = User.GetStudentId()!.Value;

        var student = await _db.Students
            .Include(s => s.ClassRoom)
            .FirstAsync(s => s.Id == studentId);

        ClassName = student.ClassRoom.ClassName;
        SectionName = student.ClassRoom.SectionName;
        AcademicYear = student.ClassRoom.AcademicYear;

        var examTypesInDb = await _db.ExamTypes.OrderBy(t => t.Id).ToListAsync();

        if (string.IsNullOrWhiteSpace(ExamTypeCode) || examTypesInDb.All(t => t.Code != ExamTypeCode))
        {
            // Default to whichever exam type actually has exams scheduled
            // for this class, preferring one flagged IsActive.
            var codesWithExams = await _db.Exams
                .Where(e => e.ClassRoomId == student.ClassRoomId)
                .Select(e => e.ExamType.Code)
                .Distinct()
                .ToListAsync();

            ExamTypeCode = examTypesInDb.FirstOrDefault(t => t.IsActive && codesWithExams.Contains(t.Code))?.Code
                ?? examTypesInDb.FirstOrDefault(t => codesWithExams.Contains(t.Code))?.Code
                ?? examTypesInDb.FirstOrDefault()?.Code
                ?? "";
        }

        ExamTypes = examTypesInDb
            .Select(t => new ExamType { Code = t.Code, Name = t.Name, Icon = t.Icon, IsActive = t.Code == ExamTypeCode })
            .ToList();

        var examRows = await _db.Exams
            .Where(e => e.ClassRoomId == student.ClassRoomId && e.ExamType.Code == ExamTypeCode)
            .OrderBy(e => e.Date)
            .ToListAsync();

        Exams = examRows
            .Select(e => new ExamItem
            {
                Date = e.Date,
                Subject = e.Subject,
                SubjectCode = e.SubjectCode ?? "",
                StartTime = e.StartTime,
                EndTime = e.EndTime,
                Room = e.Room ?? "",
                Invigilator = e.Invigilator ?? "",
                Status = e.Status,
                Icon = e.Icon
            })
            .ToList();

        ShowSubjectCode = Exams.Any(e => e.SubjectCodeAvailable);
        ShowRoom = Exams.Any(e => !string.IsNullOrWhiteSpace(e.Room));
        ShowInvigilator = Exams.Any(e => !string.IsNullOrWhiteSpace(e.Invigilator));

        var today = DateTime.Today;
        UpcomingExams = Exams.Where(x => x.Date >= today).OrderBy(x => x.Date).Take(5).ToList();

        BuildCalendar();

        await NavTrackingService.MarkViewedAsync(_db, studentId, NavSection.ExamSchedule);
    }


    // ============================================================
    // CALENDAR BUILDER
    // ============================================================

    private void BuildCalendar()
    {
        // Shows the month of the nearest upcoming exam (or, if none are
        // upcoming, the most recent past one) for the selected exam type -
        // rather than a hardcoded September 2026.
        var referenceDate = UpcomingExams.FirstOrDefault()?.Date
            ?? Exams.OrderByDescending(e => e.Date).FirstOrDefault()?.Date
            ?? DateTime.Today;

        var calendarMonth = new DateTime(referenceDate.Year, referenceDate.Month, 1);

        CurrentMonthName = calendarMonth.ToString("MMMM yyyy");

        WeekDays = new List<string>
        {
            "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
        };

        CalendarDays = new List<CalendarDay>();

        var firstDay = calendarMonth;
        var daysInMonth = DateTime.DaysInMonth(calendarMonth.Year, calendarMonth.Month);
        var startOffset = ((int)firstDay.DayOfWeek + 6) % 7;

        for (var i = startOffset - 1; i >= 0; i--)
        {
            var date = firstDay.AddDays(-(i + 1));
            CalendarDays.Add(CreateCalendarDay(date, false));
        }

        for (var day = 1; day <= daysInMonth; day++)
        {
            var date = new DateTime(calendarMonth.Year, calendarMonth.Month, day);
            CalendarDays.Add(CreateCalendarDay(date, true));
        }

        while (CalendarDays.Count % 7 != 0)
        {
            var nextDate = CalendarDays[^1].Date.AddDays(1);
            CalendarDays.Add(CreateCalendarDay(nextDate, false));
        }
    }

    private CalendarDay CreateCalendarDay(DateTime date, bool isCurrentMonth)
    {
        var exam = Exams.FirstOrDefault(x => x.Date.Date == date.Date);

        return new CalendarDay
        {
            Date = date,
            DayNumber = date.Day,
            IsCurrentMonth = isCurrentMonth,
            IsToday = date.Date == DateTime.Today,
            HasExam = exam != null,
            ExamSubject = exam?.Subject ?? ""
        };
    }


    // ============================================================
    // VIEW MODELS
    // ============================================================

    public class ExamType
    {
        public string Code { get; set; } = "";

        public string Name { get; set; } = "";

        public string Icon { get; set; } = "📅";

        public bool IsActive { get; set; }
    }


    public class ExamItem
    {
        public DateTime Date { get; set; }

        public string Subject { get; set; } = "";

        public string SubjectCode { get; set; } = "";

        public bool SubjectCodeAvailable => !string.IsNullOrWhiteSpace(SubjectCode);

        public string StartTime { get; set; } = "";

        public string EndTime { get; set; } = "";

        public string Room { get; set; } = "";

        public string Invigilator { get; set; } = "";

        public string Status { get; set; } = "";

        public string Icon { get; set; } = "📚";
    }


    public class CalendarDay
    {
        public DateTime Date { get; set; }

        public int DayNumber { get; set; }

        public bool IsCurrentMonth { get; set; }

        public bool IsToday { get; set; }

        public bool HasExam { get; set; }

        public string ExamSubject { get; set; } = "";
    }
}
