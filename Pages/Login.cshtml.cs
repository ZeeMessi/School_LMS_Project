using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages
{
    // A PageModel is the C# "code-behind" for a Razor Page — it handles
    // what happens when the browser requests the page (OnGet) or submits
    // the form on it (OnPost). Think of it as the controller for this one page.
    public class LoginModel : PageModel
    {
        // Runs on a normal page load (GET request). Nothing to do yet.
        public void OnGet()
        {
        }

        // Runs when the form is submitted (POST request). We're not checking
        // credentials yet — that comes once Entity Framework + authentication
        // are added. For now this just re-renders the page so the UI is testable.
        public IActionResult OnPost()
        {
            return Page();
        }
    }
}