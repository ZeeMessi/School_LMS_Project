namespace SchoolLMS.Data.Entities;

public class Student
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    public string RollNumber { get; set; } = "";

    // Only ever used to pick a placeholder illustration when no photo has
    // been uploaded - see Gender.cs and Services/AvatarHelper.
    public Gender Gender { get; set; }

    // Stored in the database, not on local disk - see the School.LogoData
    // comment for why, and Program.cs's /image/student/{id} endpoint for
    // how this gets served back out.
    public byte[]? PhotoData { get; set; }

    public string? PhotoContentType { get; set; }

    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    // ------------------------------------------------------------
    // GUARDIAN / CONTACT / IDENTITY DETAILS
    //
    // All optional except GuardianName - a school record normally has at
    // least one parent/guardian on file, but not every field below is
    // always known (an under-5 CNIC/B-Form isn't always issued yet, a
    // student may not have their own phone, etc). Address is plain
    // nvarchar (SQL Server's default for string columns), which stores
    // Urdu, English or digits equally well - no special handling needed.
    // ------------------------------------------------------------

    public string GuardianName { get; set; } = "";

    public string? GuardianContactNumber { get; set; }

    public string? Address { get; set; }

    public BloodGroup? BloodGroup { get; set; }

    public string? CnicOrBFormNumber { get; set; }

    public string? ContactNumber { get; set; }

    public List<AttendanceRecord> AttendanceRecords { get; set; } = new();

    public List<ExamResult> ExamResults { get; set; } = new();

    public List<FeeInvoice> FeeInvoices { get; set; } = new();
}
