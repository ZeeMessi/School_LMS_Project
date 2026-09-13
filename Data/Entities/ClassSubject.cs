namespace SchoolLMS.Data.Entities;

// A subject as taught to one specific class/section, by one teacher.
// This is what /class (Pages/class.cshtml) lists per subject card.
public class ClassSubject
{
    public int Id { get; set; }

    public int ClassRoomId { get; set; }
    public ClassRoom ClassRoom { get; set; } = null!;

    public string Name { get; set; } = "";

    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}
