namespace SchoolLMS.Data.Entities;

public class Teacher
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    // Only ever used to pick a placeholder illustration when no photo has
    // been uploaded - see Gender.cs.
    public Gender Gender { get; set; }

    // Stored in the database, not on local disk - see School.LogoData's
    // comment for why, and Program.cs's /image/teacher/{id} endpoint for
    // how this gets served back out.
    public byte[]? PhotoData { get; set; }

    public string? PhotoContentType { get; set; }

    // ------------------------------------------------------------
    // CONTACT / IDENTITY DETAILS - see the same fields on Student.cs for
    // why these are all optional and Address is left as plain nvarchar.
    // ------------------------------------------------------------

    public string? Address { get; set; }

    public BloodGroup? BloodGroup { get; set; }

    public string? CnicNumber { get; set; }

    public string? ContactNumber { get; set; }

    // The class/subject combinations this teacher is assigned to teach -
    // what their dashboard lists, and where they take attendance / enter
    // grades from.
    public List<ClassSubject> TaughtSubjects { get; set; } = new();

    public List<Announcement> Announcements { get; set; } = new();
}
