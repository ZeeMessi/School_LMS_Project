using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages;

public class ExamScheduleModel : PageModel
{
    /*
     * ============================================================
     * DATABASE-READY STRUCTURE
     * ============================================================
     *
     * The sample values below are temporary demonstration data.
     *
     * Later these properties should be populated from the school's
     * actual database/service.
     *
     * The Razor page does NOT need to be redesigned when database
     * values change.
     * ============================================================
     */


    // ------------------------------------------------------------
    // STUDENT / CLASS INFORMATION
    // ------------------------------------------------------------

    public string ClassName { get; set; } = "Class 5";

    public string SectionName { get; set; } = "Section A";

    public string AcademicYear { get; set; } = "2026 - 2027";


    // ------------------------------------------------------------
    // CURRENT EXAMINATION TYPE
    // ------------------------------------------------------------

    public string CurrentExamType { get; set; } = "QUARTERLY";


    // ------------------------------------------------------------
    // SCHOOL CONFIGURATION
    // ------------------------------------------------------------

    /*
     * These flags represent features configured by the school.
     *
     * Later these values can come directly from the database.
     */

    public bool ShowSubjectCode { get; set; } = true;

    public bool ShowRoom { get; set; } = true;

    public bool ShowInvigilator { get; set; } = true;

    public bool ShowInstructions { get; set; } = true;


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

    public int TotalSubjects =>
        Exams
            .Select(x => x.Subject)
            .Distinct()
            .Count();

    public int UpcomingExamCount =>
        UpcomingExams.Count;


    // ------------------------------------------------------------
    // EXAM INSTRUCTIONS
    // ------------------------------------------------------------

    public string ExamInstructions { get; set; } =
        "Students should arrive at least 15 minutes before the examination. " +
        "Bring the required stationery and examination materials.";


    // ============================================================
    // ON GET
    // ============================================================

    public void OnGet()
    {
        /*
         * TEMPORARY SAMPLE DATA
         *
         * Replace this section later with database/service calls.
         */

        LoadExamTypes();

        LoadExams();

        BuildCalendar();
    }


    // ============================================================
    // EXAM TYPES
    // ============================================================

    private void LoadExamTypes()
    {
        /*
         * Example:
         *
         * If a school only has Quarterly + Annual,
         * simply return those two from the database.
         *
         * No change to the .cshtml page is required.
         */

        ExamTypes = new List<ExamType>
        {
            new ExamType
            {
                Code = "QUARTERLY",
                Name = "Quarterly",
                Icon = "📝",
                IsActive = true
            },

            new ExamType
            {
                Code = "BIANNUAL",
                Name = "Bi-Annual",
                Icon = "📚",
                IsActive = false
            },

            new ExamType
            {
                Code = "ANNUAL",
                Name = "Annual",
                Icon = "🏆",
                IsActive = false
            }
        };
    }


    // ============================================================
    // EXAM DATA
    // ============================================================

    private void LoadExams()
    {
        Exams = new List<ExamItem>
        {
            new ExamItem
            {
                Date = new DateTime(2026, 9, 5),
                Subject = "English",
                SubjectCode = "ENG-05",
                StartTime = "09:00 AM",
                EndTime = "11:00 AM",
                Room = "Room 204",
                Invigilator = "Mr. Ahmed Khan",
                Status = "Upcoming",
                Icon = "🇬🇧"
            },

            new ExamItem
            {
                Date = new DateTime(2026, 9, 7),
                Subject = "Mathematics",
                SubjectCode = "MTH-05",
                StartTime = "09:00 AM",
                EndTime = "11:00 AM",
                Room = "Room 204",
                Invigilator = "Ms. Sara Ali",
                Status = "Upcoming",
                Icon = "📐"
            },

            new ExamItem
            {
                Date = new DateTime(2026, 9, 9),
                Subject = "Science",
                SubjectCode = "SCI-05",
                StartTime = "09:00 AM",
                EndTime = "11:00 AM",
                Room = "Room 204",
                Invigilator = "Mr. Bilal Ahmed",
                Status = "Upcoming",
                Icon = "🔬"
            },

            new ExamItem
            {
                Date = new DateTime(2026, 9, 11),
                Subject = "Urdu",
                SubjectCode = "URD-05",
                StartTime = "09:00 AM",
                EndTime = "11:00 AM",
                Room = "Room 204",
                Invigilator = "Mrs. Ayesha Khan",
                Status = "Upcoming",
                Icon = "📖"
            },

            new ExamItem
            {
                Date = new DateTime(2026, 9, 14),
                Subject = "Social Studies",
                SubjectCode = "SST-05",
                StartTime = "09:00 AM",
                EndTime = "11:00 AM",
                Room = "Room 205",
                Invigilator = "Mr. Hamza",
                Status = "Upcoming",
                Icon = "🌍"
            },

            new ExamItem
            {
                Date = new DateTime(2026, 9, 16),
                Subject = "Drawing",
                SubjectCode = "DRW-05",
                StartTime = "09:00 AM",
                EndTime = "10:30 AM",
                Room = "Art Room",
                Invigilator = "Ms. Hina",
                Status = "Upcoming",
                Icon = "🎨"
            }
        };


        /*
         * In the real database implementation, this could become:
         *
         * Exams = await _examService
         *     .GetExamsAsync(studentId, classId, sectionId, examTypeId);
         */


        UpcomingExams = Exams
            .Where(x => x.Date >= DateTime.Today)
            .OrderBy(x => x.Date)
            .Take(5)
            .ToList();
    }


    // ============================================================
    // CALENDAR BUILDER
    // ============================================================

    private void BuildCalendar()
    {
        /*
         * Temporary calendar month.
         *
         * Later this can be determined automatically from the
         * selected exam schedule.
         */

        var calendarMonth = new DateTime(2026, 9, 1);

        CurrentMonthName = calendarMonth.ToString("MMMM yyyy");


        WeekDays = new List<string>
        {
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday",
            "Sunday"
        };


        CalendarDays = new List<CalendarDay>();


        var firstDay = calendarMonth;

        int daysInMonth =
            DateTime.DaysInMonth(
                calendarMonth.Year,
                calendarMonth.Month);


        int startOffset =
            ((int)firstDay.DayOfWeek + 6) % 7;


        /*
         * Previous month days.
         */

        for (int i = startOffset - 1; i >= 0; i--)
        {
            var date = firstDay.AddDays(-(i + 1));

            CalendarDays.Add(
                CreateCalendarDay(
                    date,
                    false));
        }


        /*
         * Current month days.
         */

        for (int day = 1; day <= daysInMonth; day++)
        {
            var date =
                new DateTime(
                    calendarMonth.Year,
                    calendarMonth.Month,
                    day);

            CalendarDays.Add(
                CreateCalendarDay(
                    date,
                    true));
        }


        /*
         * Next month days.
         */

        while (CalendarDays.Count % 7 != 0)
        {
            var lastDate =
                CalendarDays
                    .Last()
                    .Date;

            var nextDate =
                lastDate.AddDays(1);

            CalendarDays.Add(
                CreateCalendarDay(
                    nextDate,
                    false));
        }
    }


    // ============================================================
    // CALENDAR DAY
    // ============================================================

    private CalendarDay CreateCalendarDay(
        DateTime date,
        bool isCurrentMonth)
    {
        var exam =
            Exams.FirstOrDefault(
                x => x.Date.Date == date.Date);


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
    // MODELS
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

        public bool SubjectCodeAvailable =>
            !string.IsNullOrWhiteSpace(SubjectCode);

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