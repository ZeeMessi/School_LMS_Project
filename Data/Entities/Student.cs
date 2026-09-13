namespace SchoolLMS.Data.Entities;

public class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    public string RollNumber { get; set; } = "";

    public string? PhotoUrl { get; set; }

    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    // Links this student record to a login account. Nullable for now since
    // authentication isn't wired up yet (see Pages/Login.cshtml.cs) — every
    // page currently just shows the one seeded demo student.
    public int? UserId { get; set; }

    public List<AttendanceRecord> AttendanceRecords { get; set; } = new();

    public List<ExamResult> ExamResults { get; set; } = new();

    public List<FeeInvoice> FeeInvoices { get; set; } = new();
}
