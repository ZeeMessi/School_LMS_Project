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

        public void OnGet()
        {
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
