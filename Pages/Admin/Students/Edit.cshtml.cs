using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

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
    public Gender Gender { get; set; }

    [BindProperty]
    public string GuardianName { get; set; } = "";

    [BindProperty]
    public string GuardianContactNumber { get; set; } = "";

    [BindProperty]
    public string ContactNumber { get; set; } = "";

    [BindProperty]
    public string Address { get; set; } = "";

    [BindProperty]
    public BloodGroup? BloodGroup { get; set; }

    [BindProperty]
    public string CnicOrBFormNumber { get; set; } = "";

    [BindProperty]
    public string Username { get; set; } = "";

    [BindProperty]
    public string Password { get; set; } = "";

    [BindProperty]
    public IFormFile? Photo { get; set; }

    public bool IsNew => Id is null or 0;

    public bool HasPhoto { get; set; }

    public string PlaceholderAvatarUri => AvatarHelper.PlaceholderDataUri(Gender);

    public List<ClassRoom> ClassRooms { get; set; } = new();

    public IReadOnlyList<(BloodGroup Value, string Label)> BloodGroupOptions => BloodGroupHelper.Options;

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
            Gender = student.Gender;
            GuardianName = student.GuardianName;
            GuardianContactNumber = student.GuardianContactNumber ?? "";
            ContactNumber = student.ContactNumber ?? "";
            Address = student.Address ?? "";
            BloodGroup = student.BloodGroup;
            CnicOrBFormNumber = student.CnicOrBFormNumber ?? "";
            HasPhoto = student.PhotoData != null;

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

        var photoUpload = await ImageUploadHelper.ReadAsync(Photo);
        if (photoUpload.Error is not null)
        {
            ErrorMessage = photoUpload.Error;
            return Page();
        }

        if (IsNew)
        {
            var student = new Student
            {
                FullName = FullName,
                RollNumber = RollNumber,
                ClassRoomId = ClassRoomId,
                Gender = Gender,
                GuardianName = GuardianName,
                GuardianContactNumber = NullIfBlank(GuardianContactNumber),
                ContactNumber = NullIfBlank(ContactNumber),
                Address = NullIfBlank(Address),
                BloodGroup = BloodGroup,
                CnicOrBFormNumber = NullIfBlank(CnicOrBFormNumber),
                PhotoData = photoUpload.Data,
                PhotoContentType = photoUpload.ContentType
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
            student.Gender = Gender;
            student.GuardianName = GuardianName;
            student.GuardianContactNumber = NullIfBlank(GuardianContactNumber);
            student.ContactNumber = NullIfBlank(ContactNumber);
            student.Address = NullIfBlank(Address);
            student.BloodGroup = BloodGroup;
            student.CnicOrBFormNumber = NullIfBlank(CnicOrBFormNumber);

            if (photoUpload.Data is not null)
            {
                student.PhotoData = photoUpload.Data;
                student.PhotoContentType = photoUpload.ContentType;
            }

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

    // Optional text fields are bound as "" rather than null (see the
    // model-binding note on ChallanNumber elsewhere in this codebase),
    // so an empty form field is stored as a real null instead of an
    // empty string.
    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
