
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages
{
    public class OverviewModel : PageModel
    {
        /*
         * =====================================================
         * STUDENT INFORMATION
         * =====================================================
         *
         * These are temporary values.
         *
         * In the final LMS, these values will be retrieved
         * from the database according to the logged-in user.
         */

        public string StudentName { get; set; } =
            "Ali Raza";

        public string StudentClass { get; set; } =
            "Class 5";

        public string StudentSection { get; set; } =
            "Section A";


        /*
         * =====================================================
         * ATTENDANCE
         * =====================================================
         */

        public double AttendancePercentage { get; set; } =
            92;

        public int PresentDays { get; set; } =
            184;

        public int AbsentDays { get; set; } =
            16;


        /*
         * =====================================================
         * OVERALL GRADE
         * =====================================================
         *
         * IMPORTANT:
         *
         * If the school's database does not have a grading
         * system, this value should eventually be null/empty
         * and the Grade card should not be rendered.
         */

        public string? OverallGrade { get; set; } =
            "A";


        /*
         * =====================================================
         * OVERALL PROGRESS
         * =====================================================
         */

        public double OverallProgress { get; set; } =
            78;

        public string ProgressStatus { get; set; } =
            "Good progress";


        /*
         * =====================================================
         * ACCOUNT
         * =====================================================
         *
         * If there are no outstanding dues, the account card
         * can eventually show "No Outstanding Dues".
         */

        public decimal OutstandingDues { get; set; } =
            12500;


        /*
         * =====================================================
         * UPCOMING EXAM
         * =====================================================
         *
         * If there is no upcoming examination, the exam card
         * should eventually show an appropriate empty state.
         */

        public string? UpcomingExamName { get; set; } =
            "Quarterly Examination";

        public string? UpcomingExamSubject { get; set; } =
            "Mathematics";

        public string? UpcomingExamDate { get; set; } =
            "20";

        public string? UpcomingExamMonth { get; set; } =
            "AUG";

        public string? UpcomingExamTime { get; set; } =
            "09:00 AM";


        public void OnGet()
        {
            /*
             * =================================================
             * FUTURE DATABASE CONNECTION
             * =================================================
             *
             * This method will eventually retrieve all
             * overview information from the database.
             *
             * The database should determine:
             *
             * - Student name
             * - Class
             * - Section
             * - Attendance
             * - Overall grade
             * - Overall progress
             * - Outstanding dues
             * - Upcoming examination
             *
             * The UI should NOT contain school-specific
             * hard-coded information in the final version.
             */
        }
    }
}

