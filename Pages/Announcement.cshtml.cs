using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SchoolLMS.Pages
{
    public class AnnouncementModel : PageModel
    {
        public List<AnnouncementItem> Announcements { get; set; }
            = new List<AnnouncementItem>();


        public void OnGet()
        {
            /*
             * TEMPORARY SAMPLE DATA
             *
             * This data is only being used to test the UI.
             *
             * Later:
             * Replace this section with database/service retrieval.
             *
             * No announcement type is permanently hard-coded into
             * the page structure.
             */

            Announcements = new List<AnnouncementItem>
            {
                new AnnouncementItem
                {
                    Id = 1,
                    Title = "Parent-Teacher Meeting",
                    Category = "Notice",
                    Date = new DateTime(2026, 8, 13),
                    ShortDescription =
                        "Parents are requested to attend the upcoming parent-teacher meeting.",
                    FullDescription =
                        "The school administration has announced a parent-teacher meeting. Parents will receive further details regarding the meeting schedule and venue.",
                    IssuedBy = "School Administration",
                    IsImportant = true,
                    IsRead = false
                },

                new AnnouncementItem
                {
                    Id = 2,
                    Title = "Annual Examination Schedule",
                    Category = "Notice",
                    Date = new DateTime(2026, 8, 10),
                    ShortDescription =
                        "The annual examination schedule has been announced.",
                    FullDescription =
                        "The annual examination schedule has been published by the school administration. Students should check the examination schedule carefully.",
                    IssuedBy = "Examination Department",
                    IsImportant = true,
                    IsRead = false
                },

                new AnnouncementItem
                {
                    Id = 3,
                    Title = "School Activity",
                    Category = "Event",
                    Date = new DateTime(2026, 8, 7),
                    ShortDescription =
                        "A school activity has been scheduled for students.",
                    FullDescription =
                        "Students are requested to participate in the scheduled school activity.",
                    IssuedBy = "School Administration",
                    IsImportant = false,
                    IsRead = true
                },

                new AnnouncementItem
                {
                    Id = 4,
                    Title = "School Holiday Notice",
                    Category = "Notice",
                    Date = new DateTime(2026, 8, 3),
                    ShortDescription =
                        "The school will remain closed on the announced holiday.",
                    FullDescription =
                        "The school administration has announced a holiday. Regular classes will resume according to the school timetable.",
                    IssuedBy = "School Administration",
                    IsImportant = false,
                    IsRead = true
                }
            };
        }
    }


    /*
     * ============================================================
     * ANNOUNCEMENT DATA MODEL
     * ============================================================
     *
     * These properties are intentionally separated so that the
     * database can provide them later without changing the UI.
     */

    public class AnnouncementItem
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string ShortDescription { get; set; } = string.Empty;

        public string FullDescription { get; set; } = string.Empty;

        public string IssuedBy { get; set; } = string.Empty;

        public bool IsImportant { get; set; }

        public bool IsRead { get; set; }

        /*
         * Future database fields can be added here:
         *
         * AttachmentUrl
         * AttachmentName
         * ImageUrl
         * TargetClass
         * TargetSection
         * PublishedDate
         * ExpiryDate
         * etc.
         */
    }
}