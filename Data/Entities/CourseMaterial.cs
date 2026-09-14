namespace SchoolLMS.Data.Entities;

// An assignment, handout, or quiz a teacher posted for one class/subject -
// see Pages/Teacher/Materials.cshtml.cs (where these get created) and
// Pages/SubjectContent.cshtml.cs (where a student sees them). The file is
// optional - a teacher can post a text-only assignment with no attachment.
public class CourseMaterial
{
    public int Id { get; set; }

    public int ClassSubjectId { get; set; }
    public ClassSubject ClassSubject { get; set; } = null!;

    public MaterialType Type { get; set; }

    public string Title { get; set; } = "";

    public string Description { get; set; } = "";

    public DateOnly? DueDate { get; set; }

    public DateTime PostedAt { get; set; }

    // Stored in the database like every other upload in this app (see
    // School.LogoData's comment) - not on local disk.
    public byte[]? FileData { get; set; }

    public string? FileContentType { get; set; }

    public string? FileName { get; set; }
}
