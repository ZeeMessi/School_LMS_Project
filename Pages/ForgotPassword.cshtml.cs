using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;

namespace SchoolLMS.Pages;

// No email/SMS sending is set up in this app, so there's no self-service
// reset flow yet - this page just explains the real path: an Admin account
// can reset anyone's password from Admin > Students/Teachers > Edit (see
// Pages/Admin/Students/Edit.cshtml.cs and Pages/Admin/Teachers/Edit.cshtml.cs),
// which already has a "New Password" field for exactly this.
public class ForgotPasswordModel : PageModel
{
    private readonly AppDbContext _db;

    public ForgotPasswordModel(AppDbContext db)
    {
        _db = db;
    }

    public bool HasSchoolLogo { get; set; }

    public async Task OnGetAsync()
    {
        HasSchoolLogo = await _db.Schools.AnyAsync(s => s.LogoData != null);
    }
}
