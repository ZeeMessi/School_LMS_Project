using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SchoolLMS.Data;
using SchoolLMS.Data.Entities;
using SchoolLMS.Services;

namespace SchoolLMS.Pages
{
    // See Overview.cshtml.cs - reads User.GetStudentId()!.Value, so must
    // stay restricted to the Student role.
    [Authorize(Roles = "Student")]
    public class FeedbackModel : PageModel
    {
        private readonly AppDbContext _db;

        public FeedbackModel(AppDbContext db)
        {
            _db = db;
        }

        [BindProperty]
        public FeedbackInput Feedback { get; set; } = new();

        public string? StatusMessage { get; private set; }


        public void OnGet()
        {
            // No login yet, so submissions aren't tied to a real logged-in
            // student/parent - see OnPostAsync below.
        }


        public async Task OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return;
            }

            var studentId = User.GetStudentId()!.Value;
            var student = await _db.Students.FirstAsync(s => s.Id == studentId);

            _db.FeedbackSubmissions.Add(new FeedbackSubmission
            {
                StudentId = student.Id,
                Category = Feedback.Category!,
                Rating = Feedback.Rating!.Value,
                Message = Feedback.Message!,
                SubmittedAt = DateTime.UtcNow,
                Status = "New"
            });

            await _db.SaveChangesAsync();

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
