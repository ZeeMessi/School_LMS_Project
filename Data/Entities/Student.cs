namespace SchoolLMS.Data.Entities;

public class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    public string RollNumber { get; set; } = "";

    // Stored in the database, not on local disk - see the School.LogoData
    // comment for why, and Program.cs's /image/student/{id} endpoint for
    // how this gets served back out.
    public byte[]? PhotoData { get; set; }

    public string? PhotoContentType { get; set; }

    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public List<AttendanceRecord> AttendanceRecords { get; set; } = new();

    public List<ExamResult> ExamResults { get; set; } = new();

    public List<FeeInvoice> FeeInvoices { get; set; } = new();
}
