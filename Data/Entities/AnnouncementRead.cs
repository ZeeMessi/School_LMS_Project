namespace SchoolLMS.Data.Entities;

// "Read" is per-viewer, not a property of the announcement itself, so it's
// its own join row rather than a bool on Announcement. Only becomes
// meaningful once Login is wired to a real student account (see
// Student.UserId) — until then, pages can treat everything as unread.
public class AnnouncementRead
{
    public int Id { get; set; }

    public int AnnouncementId { get; set; }
    public Announcement Announcement { get; set; } = null!;

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public DateTime ReadAt { get; set; }
}
