namespace SchoolLMS.Data.Entities;

// A teacher's remark about one student in one subject - what the "Teacher
// Remarks" option on the student's Class page shows, and what
// Pages/Teacher/Remarks.cshtml.cs lets a teacher write. Distinct from
// ExamResult.Remarks (a note tied to one specific exam) - this is a
// general, non-exam-specific comment, and there can be several over time.
public class TeacherRemark
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int ClassSubjectId { get; set; }
    public ClassSubject ClassSubject { get; set; } = null!;

    public string Remark { get; set; } = "";

    public DateTime CreatedAt { get; set; }
}
