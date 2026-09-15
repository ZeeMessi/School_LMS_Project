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

    // When this row was actually entered (by an admin publishing a
    // schedule, or a teacher recording a result) - distinct from Date,
    // the exam's own scheduled date, which can be well in the future.
    // Services/NavTrackingService compares this against the student's
    // last visit to Exam Schedule to decide whether to show a "new" badge.
    public DateTime CreatedAt { get; set; }

    public List<ExamResult> Results { get; set; } = new();
}
