using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace School_LMS.Pages
{
    public class FeedbackModel : PageModel
    {
        [BindProperty]
        public FeedbackInput Feedback { get; set; } = new();

        public string? StatusMessage { get; private set; }


        public void OnGet()
        {
            // Future:
            // Load logged-in student/parent information
            // from the database here.
        }


        public void OnPost()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            /*
             * DATABASE INTEGRATION WILL GO HERE.
             *
             * Future database flow:
             *
             * 1. Identify the logged-in student/parent.
             * 2. Save Category.
             * 3. Save Rating.
             * 4. Save Message.
             * 5. Save submission date/time.
             * 6. Save submission status.
             *
             * Example future fields:
             *
             * StudentId
             * ParentId
             * Category
             * Rating
             * Message
             * SubmittedAt
             * Status
             */


            // Temporary response until database is connected.
            StatusMessage =
                "Thank you. Your feedback has been submitted successfully.";

            // Clear the form after successful submission.
            Feedback = new FeedbackInput();
        }


        public class FeedbackInput
        {
            [Required(ErrorMessage = "Please select a feedback category.")]
            public string? Category { get; set; }


            [Range(
                1,
                5,
                ErrorMessage = "Please select a rating."
            )]
            public int? Rating { get; set; }


            [Required(
                ErrorMessage = "Please enter your feedback."
            )]

            [StringLength(
                1000,
                MinimumLength = 5,
                ErrorMessage =
                    "Feedback must be between 5 and 1000 characters."
            )]

            public string? Message { get; set; }
        }
    }
}