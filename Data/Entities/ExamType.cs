namespace SchoolLMS.Data.Entities;

// Only the exam types a given school actually configures show up in
// Pages/ExamSchedule.cshtml's selector — e.g. a school with only
// Quarterly + Annual just won't seed the other rows.
public class ExamType
{
    public int Id { get; set; }

    public string Code { get; set; } = "";

    public string Name { get; set; } = "";

    public string Icon { get; set; } = "📅";

    public bool IsActive { get; set; }

    public List<Exam> Exams { get; set; } = new();
}
