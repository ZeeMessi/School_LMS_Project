using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages;

// No email/SMS sending is set up in this app, so there's no self-service
// reset flow yet - this page just explains the real path: an Admin account
// can reset anyone's password from Admin > Students/Teachers > Edit (see
// Pages/Admin/Students/Edit.cshtml.cs and Pages/Admin/Teachers/Edit.cshtml.cs),
// which already has a "New Password" field for exactly this.
public class ForgotPasswordModel : PageModel
{
    public void OnGet()
    {
    }
}
