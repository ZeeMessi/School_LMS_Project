namespace SchoolLMS.Data.Entities;

public class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    public string RollNumber { get; set; } = "";

    public string? PhotoUrl { get; set; }

    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public List<AttendanceRecord> AttendanceRecords { get; set; } = new();

    public List<ExamResult> ExamResults { get; set; } = new();

    public List<FeeInvoice> FeeInvoices { get; set; } = new();
}
