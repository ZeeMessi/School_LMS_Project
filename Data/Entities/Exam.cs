namespace SchoolLMS.Data.Entities;

// One scheduled examination for one subject/class — what
// Pages/ExamSchedule.cshtml lists in its calendar and timetable.
public class Exam
{
    public int Id { get; set; }

    public int ExamTypeId { get; set; }
    public ExamType ExamType { get; set; } = null!;

    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public string Subject { get; set; } = "";

    public string? SubjectCode { get; set; }

    public string Icon { get; set; } = "📚";

    public DateTime Date { get; set; }

    public string StartTime { get; set; } = "";

    public string EndTime { get; set; } = "";

    public string? Room { get; set; }

    public string? Invigilator { get; set; }

    public string Status { get; set; } = "Upcoming";

    public List<ExamResult> Results { get; set; } = new();
}
