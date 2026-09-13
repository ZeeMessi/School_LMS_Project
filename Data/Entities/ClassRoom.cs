namespace SchoolLMS.Data.Entities;

// A single class + section, e.g. "Class 5" / "Section A", for one academic
// year. Named ClassRoom (not "Class") to avoid colliding with the C# keyword
// and with the existing Pages/class.cshtml.cs ClassModel.
public class ClassRoom
{
    public int Id { get; set; }

    public string ClassName { get; set; } = "";

    public string SectionName { get; set; } = "";

    public string AcademicYear { get; set; } = "";

    public List<Student> Students { get; set; } = new();

    public List<ClassSubject> Subjects { get; set; } = new();
}
