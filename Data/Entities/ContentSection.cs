namespace SchoolLMS.Data.Entities;

// The five per-subject boxes on the student's Class page (see
// class.cshtml) and the matching tabs on SubjectContent.cshtml. Used by
// StudentSectionView to track, per student and per subject, when each
// one was last opened.
public enum ContentSection
{
    Assignment,
    Handout,
    Quiz,
    Remark,
    Announcement
}
