using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;
// SchoolLMS.Pages.Teacher is a real namespace (the Teacher portal pages)
// that would otherwise shadow the real entity type here, since
// enclosing-namespace lookup wins over `using`.
using TeacherEntity = SchoolLMS.Data.Entities.Teacher;

namespace SchoolLMS.Pages.Admin.Teachers;

[Authorize(Roles = "Admin")]
public class EditModel : PageModel
{
    private readonly AppDbContext _db;
    private readonly IPasswordHasher<UserAccount> _passwordHasher;

    public EditModel(AppDbContext db, IPasswordHasher<UserAccount> passwordHasher)
    {
        _db = db;
        _passwordHasher = passwordHasher;
    }

    [BindProperty(SupportsGet = true)]
    public int? Id { get; set; }

    [BindProperty]
    public string FullName { get; set; } = "";

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    [BindProperty]
    public IFormFile? Photo { get; set; }

    public bool IsNew => Id is null or 0;

    public bool HasPhoto { get; set; }

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        if (!IsNew)
        {
            var teacher = await _db.Teachers.FirstAsync(t => t.Id == Id);
            FullName = teacher.FullName;
            HasPhoto = teacher.PhotoData != null;

            var account = await _db.UserAccounts.FirstOrDefaultAsync(u => u.TeacherId == Id);
            Username = account?.Username ?? "";
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Name and username are both required.";
            return Page();
        }

        if (IsNew && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "A password is required for a new account.";
            return Page();
        }

        var usernameTaken = await _db.UserAccounts.AnyAsync(u =>
            u.Username == Username && (IsNew || u.TeacherId != Id));

        if (usernameTaken)
        {
            ErrorMessage = "That username is already in use.";
            return Page();
        }

        var photoUpload = await ImageUploadHelper.ReadAsync(Photo);
        if (photoUpload.Error is not null)
        {
            ErrorMessage = photoUpload.Error;
            return Page();
        }

        if (IsNew)
        {
            var teacher = new TeacherEntity
            {
                FullName = FullName,
                PhotoData = photoUpload.Data,
                PhotoContentType = photoUpload.ContentType
            };
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync(); // assigns teacher.Id

            var account = new UserAccount
            {
                Username = Username,
                Role = UserRole.Teacher,
                TeacherId = teacher.Id
            };
            account.PasswordHash = _passwordHasher.HashPassword(account, Password);
            _db.UserAccounts.Add(account);
        }
        else
        {
            var teacher = await _db.Teachers.FirstAsync(t => t.Id == Id);
            teacher.FullName = FullName;

            if (photoUpload.Data is not null)
            {
                teacher.PhotoData = photoUpload.Data;
                teacher.PhotoContentType = photoUpload.ContentType;
            }

            var account = await _db.UserAccounts.FirstOrDefaultAsync(u => u.TeacherId == Id);
            if (account is null)
            {
                account = new UserAccount { Role = UserRole.Teacher, TeacherId = Id };
                _db.UserAccounts.Add(account);
            }

            account.Username = Username;
            if (!string.IsNullOrWhiteSpace(Password))
            {
                account.PasswordHash = _passwordHasher.HashPassword(account, Password);
            }
        }

        await _db.SaveChangesAsync();
        return RedirectToPage("Index");
    }
}
