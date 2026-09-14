using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Services;

namespace SchoolLMS.Pages.Admin.School;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly AppDbContext _db;

    public IndexModel(AppDbContext db)
    {
        _db = db;
    }

    [BindProperty]
    public string Name { get; set; } = "";

    [BindProperty]
    public string AboutUs { get; set; } = "";

    [BindProperty]
    public string Address { get; set; } = "";

    [BindProperty]
    public string Dial { get; set; } = "";

    [BindProperty]
    public string Mobile { get; set; } = "";

    [BindProperty]
    public string Fax { get; set; } = "";

    [BindProperty]
    public string Email { get; set; } = "";

    [BindProperty]
    public IFormFile? Logo { get; set; }

    public bool HasLogo { get; set; }

    public string? ErrorMessage { get; set; }

    [TempData]
    public string? StatusMessage { get; set; }

    public async Task OnGetAsync()
    {
        var school = await _db.Schools.FirstAsync();
        Name = school.Name;
        AboutUs = school.AboutUs;
        Address = school.Address;
        Dial = school.Dial;
        Mobile = school.Mobile;
        Fax = school.Fax;
        Email = school.Email;
        HasLogo = school.LogoData != null;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var school = await _db.Schools.FirstAsync();

        if (string.IsNullOrWhiteSpace(Name))
        {
            ErrorMessage = "School name is required.";
            HasLogo = school.LogoData != null;
            return Page();
        }

        var upload = await ImageUploadHelper.ReadAsync(Logo);
        if (upload.Error is not null)
        {
            ErrorMessage = upload.Error;
            HasLogo = school.LogoData != null;
            return Page();
        }

        school.Name = Name;
        school.AboutUs = AboutUs;
        school.Address = Address;
        school.Dial = Dial;
        school.Mobile = Mobile;
        school.Fax = Fax;
        school.Email = Email;

        if (upload.Data is not null)
        {
            school.LogoData = upload.Data;
            school.LogoContentType = upload.ContentType;
        }

        await _db.SaveChangesAsync();

        StatusMessage = "School details saved.";
        return RedirectToPage();
    }
}
