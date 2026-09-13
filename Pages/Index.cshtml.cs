using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages;

// The app's root route ("/") just forwards to the real dashboard page.
// Overview.cshtml is the actual, fully-built landing page — no need to
// duplicate its content here.
public class IndexModel : PageModel
{
    public IActionResult OnGet()
    {
        return RedirectToPage("/Overview");
    }
}
