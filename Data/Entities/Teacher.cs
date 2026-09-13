namespace SchoolLMS.Data.Entities;

public class Teacher
{
    public int Id { get; set; }

    public string FullName { get; set; } = "";

    public string? PhotoUrl { get; set; }

    // The class/subject combinations this teacher is assigned to teach -
    // what their dashboard lists, and where they take attendance / enter
    // grades from.
    public List<ClassSubject> TaughtSubjects { get; set; } = new();
}
