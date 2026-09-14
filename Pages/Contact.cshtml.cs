using Microsoft.AspNetCore.Mvc.RazorPages;
using SchoolLMS.Data;

namespace SchoolLMS.Pages
{
    public class ContactModel : PageModel
    {
        private readonly AppDbContext _db;

        public ContactModel(AppDbContext db)
        {
            _db = db;
        }

        public string SchoolName { get; set; } = "";

        public string? SchoolLogo { get; set; }

        public string AboutUs { get; set; } = "";

        public string Address { get; set; } = "";

        public string Dial { get; set; } = "";

        public string Mobile { get; set; } = "";

        public string Fax { get; set; } = "";

        public string Email { get; set; } = "";

        public void OnGet()
        {
            // One database per school deployment means there's only ever
            // one School row here — this is simply that school's profile.
            var school = _db.Schools.First();

            SchoolName = school.Name;
            SchoolLogo = school.LogoData != null ? "/image/school" : null;
            AboutUs = school.AboutUs;
            Address = school.Address;
            Dial = school.Dial;
            Mobile = school.Mobile;
            Fax = school.Fax;
            Email = school.Email;
        }
    }
}
