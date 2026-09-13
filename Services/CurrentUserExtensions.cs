using System.Security.Claims;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Services;

// Small helpers for reading the logged-in user's identity out of the
// cookie-auth claims set at sign-in (see Pages/Login.cshtml.cs). Kept as
// extension methods on ClaimsPrincipal so every PageModel can just call
// User.GetStudentId() etc. instead of repeating claim-lookup code.
public static class CurrentUserExtensions
{
    public const string StudentIdClaim = "StudentId";
    public const string TeacherIdClaim = "TeacherId";

    public static int? GetStudentId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(StudentIdClaim);
        return int.TryParse(value, out var id) ? id : null;
    }

    public static int? GetTeacherId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(TeacherIdClaim);
        return int.TryParse(value, out var id) ? id : null;
    }

    public static UserRole? GetRole(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.Role);
        return Enum.TryParse<UserRole>(value, out var role) ? role : null;
    }
}
