using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    public class LoginModel : PageModel
    {
        private readonly AppDbContext _db;
        private readonly IPasswordHasher<UserAccount> _passwordHasher;

        public LoginModel(AppDbContext db, IPasswordHasher<UserAccount> passwordHasher)
        {
            _db = db;
            _passwordHasher = passwordHasher;
        }

        [BindProperty]
        public string Username { get; set; } = "";

        [BindProperty]
        public string Password { get; set; } = "";

        public string? ErrorMessage { get; set; }

        public string SchoolName { get; set; } = "";

        public bool HasSchoolLogo { get; set; }

        public async Task OnGetAsync()
        {
            // One database per school deployment means there's only ever
            // one School row - see Contact.cshtml.cs. Anyone can reach the
            // login page without being signed in yet, so this is one of
            // the few places that reads school info before auth exists.
            var school = await _db.Schools.FirstOrDefaultAsync();
            SchoolName = school?.Name ?? "Your School Name";
            HasSchoolLogo = school?.LogoData != null;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var account = await _db.UserAccounts
                .FirstOrDefaultAsync(u => u.Username == Username && u.IsActive);

            // Same error for "no such user" and "wrong password" - not
            // revealing which one it was is standard practice, not an
            // oversight.
            if (account is null ||
                _passwordHasher.VerifyHashedPassword(account, account.PasswordHash, Password)
                    == PasswordVerificationResult.Failed)
            {
                ErrorMessage = "Invalid username or password.";

                // OnPostAsync's own Page() result doesn't run OnGetAsync
                // first, so the school name/logo need loading here too, or
                // a failed login attempt would redisplay the form with the
                // "Your School Name" fallback even when a logo is set.
                var school = await _db.Schools.FirstOrDefaultAsync();
                SchoolName = school?.Name ?? "Your School Name";
                HasSchoolLogo = school?.LogoData != null;

                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new(ClaimTypes.Name, account.Username),
                new(ClaimTypes.Role, account.Role.ToString())
            };

            if (account.StudentId is { } studentId)
            {
                claims.Add(new Claim(CurrentUserExtensions.StudentIdClaim, studentId.ToString()));
            }

            if (account.TeacherId is { } teacherId)
            {
                claims.Add(new Claim(CurrentUserExtensions.TeacherIdClaim, teacherId.ToString()));
            }

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity));

            return account.Role switch
            {
                UserRole.Student => RedirectToPage("/Overview"),
                UserRole.Teacher => RedirectToPage("/Teacher/Dashboard"),
                UserRole.Admin => RedirectToPage("/Admin/Dashboard"),
                _ => RedirectToPage("/Login")
            };
        }
    }
}
