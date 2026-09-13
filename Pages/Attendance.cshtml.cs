using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages
{
    public class AttendanceModel : PageModel
    {
        public void OnGet()
        {
            // =====================================================
            // TEMPORARY FRONTEND TEST
            //
            // IMPORTANT:
            // The attendance information currently displayed by
            // Attendance.cshtml is sample data.
            //
            // Later this PageModel will retrieve:
            //
            // - Student
            // - Class
            // - Section
            // - Dates
            // - Attendance status
            // - Holidays
            // - Monthly totals
            // - Attendance percentage
            //
            // from the database.
            //
            // The HTML structure should NOT need to be redesigned
            // when the database is connected.
            // =====================================================
        }
    }
}