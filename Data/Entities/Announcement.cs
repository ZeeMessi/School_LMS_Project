namespace SchoolLMS.Data.Entities;

// Backs Pages/Announcement.cshtml. TargetClassRoomId is nullable so a
// school-wide announcement (null) can coexist with class-specific ones.
public class Announcement
{
    public int Id { get; set; }

    public string Title { get; set; } = "";

    public string Category { get; set; } = "";

    public DateTime Date { get; set; }

    public string ShortDescription { get; set; } = "";

    public string FullDescription { get; set; } = "";

    public string IssuedBy { get; set; } = "";

    public bool IsImportant { get; set; }

    public int? TargetClassRoomId { get; set; }
    public ClassRoom? TargetClassRoom { get; set; }

    // Null for announcements seeded/posted by the school itself (see
    // DbSeeder) rather than a specific teacher - IssuedBy still holds the
    // display text ("School Administration", etc.) either way. This is
    // what lets Pages/Teacher/Announcements.cshtml.cs list "my own posts"
    // separately from everything else.
    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    // Set only for an announcement Admin/Exams generated automatically
    // when publishing/updating an exam schedule (see
    // Pages/Admin/Exams/Index.cshtml.cs) - lets that page find "the"
    // announcement for a given exam type/class to update in place
    // instead of posting a new one every time a subject is added to the
    // same schedule.
    public int? RelatedExamTypeId { get; set; }
    public ExamType? RelatedExamType { get; set; }

    // Stored in the database like every other upload in this app (see
    // School.LogoData's comment) - replaces the old AttachmentUrl string,
    // which nothing had ever actually set.
    public byte[]? AttachmentData { get; set; }

    public string? AttachmentContentType { get; set; }

    public string? AttachmentFileName { get; set; }
}
