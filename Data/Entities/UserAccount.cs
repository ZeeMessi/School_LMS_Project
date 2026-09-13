namespace SchoolLMS.Data.Entities;

// The real login account behind Pages/Login.cshtml. One account per person,
// regardless of role - Username is the single login field for everyone
// (students, teachers, and admins alike), matching the log-in screen.
public class UserAccount
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    public string PasswordHash { get; set; } = "";

    public UserRole Role { get; set; }

    public bool IsActive { get; set; } = true;

    // Exactly one of these is set, depending on Role. Both null for Admin
    // accounts, which aren't tied to a student/teacher record.
    public int? StudentId { get; set; }
    public Student? Student { get; set; }

    public int? TeacherId { get; set; }
    public Teacher? Teacher { get; set; }
}
