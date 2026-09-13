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

    public string? AttachmentUrl { get; set; }
}
