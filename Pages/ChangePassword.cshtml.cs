using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Pages;

// Available to every signed-in role (Student/Teacher/Admin) - the one
// self-service account action every role needs. Most importantly, this
// is the ONLY way an Admin can change their own password: Admin >
// Students/Teachers > Edit only ever reset OTHER people's passwords,
// and DbSeeder.SeedProductionDefaults hands a real deployment's first
// Admin login a randomly-generated password with nowhere else to
// change it from.
[Authorize]
public class ChangePasswordModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public ChangePasswordModel(AppDbContext db, IPasswordHasher<UserAccount> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty]
    public string CurrentPassword { get; set; } = "";

    [BindProperty]
    public string NewPassword { get; set; } = "";

    [BindProperty]
    public string ConfirmPassword { get; set; } = "";

    public string Username { get; private set; } = "";

    public string? ErrorMessage { get; set; }

    public string? StatusMessage { get; set; }

    public void OnGet()
    {
        Username = User.Identity?.Name ?? "";
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Username = User.Identity?.Name ?? "";

        var account = await GetCurrentAccountAsync();

        if (_passwordHasher.VerifyHashedPassword(account, account.PasswordHash, CurrentPassword)
            == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Current password is incorrect.";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
        {
            ErrorMessage = "New password must be at least 6 characters.";
            return Page();
        }

        if (NewPassword != ConfirmPassword)
        {
            ErrorMessage = "New password and confirmation don't match.";
            return Page();
        }

        account.PasswordHash = _passwordHasher.HashPassword(account, NewPassword);
        await _db.SaveChangesAsync();

        StatusMessage = "Password updated.";
        CurrentPassword = "";
        NewPassword = "";
        ConfirmPassword = "";
        return Page();
    }

    private async Task<UserAccount> GetCurrentAccountAsync()
    {
        var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        return await _db.UserAccounts.FirstAsync(u => u.Id == accountId);
    }
}
