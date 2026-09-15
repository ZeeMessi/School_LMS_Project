namespace SchoolLMS.Data.Entities;

// The four sidebar tabs on Pages/Shared/_Layout.cshtml that have their
// own "last viewed" tracking (see StudentNavView and
// Services/NavTrackingService.NavFlags). Class and Overview also show a
// badge, but as a computed OR of these plus the per-subject flags from
// ContentTrackingService - there's nothing distinct to mark "viewed" on
// either of those two.
public enum NavSection
{
    Announcement,
    ExamSchedule,
    GradeBook,
    AccountBook
}
