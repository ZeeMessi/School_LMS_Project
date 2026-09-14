using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;

namespace SchoolLMS.Pages.Admin.Students;

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
    public string RollNumber { get; set; } = "";

    [BindProperty]
    public int ClassRoomId { get; set; }

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    public bool IsNew => Id is null or 0;

    public List<ClassRoom> ClassRooms { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        ClassRooms = await _db.ClassRooms.OrderBy(c => c.ClassName).ThenBy(c => c.SectionName).ToListAsync();

        if (!IsNew)
        {
            var student = await _db.Students.FirstAsync(s => s.Id == Id);
            FullName = student.FullName;
            RollNumber = student.RollNumber;
            ClassRoomId = student.ClassRoomId;

            var account = await _db.UserAccounts.FirstOrDefaultAsync(u => u.StudentId == Id);
            Username = account?.Username ?? "";
        }
        else if (ClassRooms.Count > 0)
        {
            ClassRoomId = ClassRooms[0].Id;
        }
    }

    public async Task<IActionResult> OnPostAsync()
    {
        ClassRooms = await _db.ClassRooms.OrderBy(c => c.ClassName).ThenBy(c => c.SectionName).ToListAsync();

        if (string.IsNullOrWhiteSpace(FullName) || string.IsNullOrWhiteSpace(RollNumber) || string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Name, roll number, and username are all required.";
            return Page();
        }

        if (IsNew && string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "A password is required for a new account.";
            return Page();
        }

        var usernameTaken = await _db.UserAccounts.AnyAsync(u =>
            u.Username == Username && (IsNew || u.StudentId != Id));

        if (usernameTaken)
        {
            ErrorMessage = "That username is already in use.";
            return Page();
        }

        if (IsNew)
        {
            var student = new Student
            {
                FullName = FullName,
                RollNumber = RollNumber,
                ClassRoomId = ClassRoomId
            };
            _db.Students.Add(student);
            await _db.SaveChangesAsync(); // assigns student.Id

            var account = new UserAccount
            {
                Username = Username,
                Role = UserRole.Student,
                StudentId = student.Id
            };
            account.PasswordHash = _passwordHasher.HashPassword(account, Password);
            _db.UserAccounts.Add(account);
        }
        else
        {
            var student = await _db.Students.FirstAsync(s => s.Id == Id);
            student.FullName = FullName;
            student.RollNumber = RollNumber;
            student.ClassRoomId = ClassRoomId;

            var account = await _db.UserAccounts.FirstOrDefaultAsync(u => u.StudentId == Id);
            if (account is null)
            {
                account = new UserAccount { Role = UserRole.Student, StudentId = Id };
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
