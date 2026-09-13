namespace SchoolLMS.Data.Entities;

// Backs Pages/Feedback.cshtml. StudentId is nullable in case feedback is
// ever allowed without being logged in, though today's flow assumes a
// logged-in student.
public class FeedbackSubmission
{
    public int Id { get; set; }

    public int? StudentId { get; set; }
    public Student? Student { get; set; }

    public string Category { get; set; } = "";

    public int Rating { get; set; }

    public string Message { get; set; } = "";

    public DateTime SubmittedAt { get; set; }

    public string Status { get; set; } = "New";
}
