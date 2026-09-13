using Microsoft.AspNetCore.Mvc.RazorPages;

namespace School_LMS.Pages
{
    public class ContactModel : PageModel
    {
        /*
         * =====================================================
         * FUTURE DATABASE DATA
         * =====================================================
         *
         * These values will eventually come from the school's
         * database.
         *
         * The page structure does not need to change when the
         * school changes its information.
         */

        public string SchoolName { get; set; } =
            "School Name";

        public string SchoolLogo { get; set; } =
            "";

        public string AboutUs { get; set; } =
            "School information will appear here.";

        public string Address { get; set; } =
            "School Address";

        public string Dial { get; set; } =
            "+92 (51-111-8-88-80)";

        public string Mobile { get; set; } =
            "0304-1234567";

        public string Fax { get; set; } =
            "+92 (51) 1234567";

        public string Email { get; set; } =
            "school@example.com";


        public void OnGet()
        {
            /*
             * FUTURE:
             *
             * Load school information from database here.
             *
             * Example:
             *
             * SchoolName = school.Name;
             * SchoolLogo = school.LogoUrl;
             * Address = school.Address;
             * Dial = school.Dial;
             * Mobile = school.Mobile;
             * Fax = school.Fax;
             * Email = school.Email;
             * AboutUs = school.AboutUs;
             */
        }
    }
}
