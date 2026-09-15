namespace SchoolLMS.Data.Entities;

// One student's result for one exam — backs both Pages/Grade.cshtml
// (per-subject rows) and the "Overall Grade" card on Pages/Overview.cshtml.
public class ExamResult
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int TotalMarks { get; set; }

    public int ObtainedMarks { get; set; }

    public string Grade { get; set; } = "";

    public string? Remarks { get; set; }

    // When this result was recorded - lets Services/NavTrackingService
    // show a "new" badge on Grade Book when a teacher posts new marks.
    public DateTime RecordedAt { get; set; }
}
