namespace SchoolLMS.Data.Entities;

// Same idea as StudentSectionView, but for whole sidebar tabs (see
// NavSection) rather than one subject's Assignment/Handout/Quiz/Remark/
// Announcement boxes - powers the "new" dot on Pages/Shared/_Layout.cshtml's
// sidebar links, via Services/NavTrackingService.
public class StudentNavView
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public NavSection Section { get; set; }

    public DateTime LastViewedAt { get; set; }
}
