namespace SchoolLMS.Data.Entities;

public enum AttendanceStatus
{
    Present,
    Absent,
    Weekend,
    Holiday,
    NotRecorded
}

public class AttendanceRecord
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public DateOnly Date { get; set; }

    public AttendanceStatus Status { get; set; }
}
