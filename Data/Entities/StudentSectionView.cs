namespace SchoolLMS.Data.Entities;

// Records the last time a student opened one of the five per-subject
// content boxes (Assignment/Handout/Quiz/Remark/Announcement, see
// ContentSection) for a given ClassSubject. Comparing this against the
// newest item actually posted in that section is what lets the "new"
// badge on class.cshtml and SubjectContent.cshtml be a real signal
// instead of always being on - see Services/ContentTrackingService.
public class StudentSectionView
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int ClassSubjectId { get; set; }
    public ClassSubject ClassSubject { get; set; } = null!;

    public ContentSection Section { get; set; }

    public DateTime LastViewedAt { get; set; }
}
