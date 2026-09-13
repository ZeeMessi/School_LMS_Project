using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;

namespace SchoolLMS.Pages
{
    public class ClassModel : PageModel
    {
        // =========================================================
        // TEMPORARY CLASS INFORMATION
        // Later these values will come from the database.
        // =========================================================

        public string ClassName { get; set; } = "Class 5";

        public string SectionName { get; set; } = "Section A";


        // =========================================================
        // SUBJECT LIST
        // =========================================================

        public List<ClassSubject> Subjects { get; set; } = new();


        // =========================================================
        // OPTIONS SHOWN INSIDE EACH SUBJECT CARD
        // =========================================================

        public List<ClassSubjectOption> SubjectOptions { get; set; } = new()
        {
            new ClassSubjectOption
            {
                Name = "Assignment",
                Icon = ""
            },

            new ClassSubjectOption
            {
                Name = "Quiz",
                Icon = ""
            },

            new ClassSubjectOption
            {
                Name = "Handouts",
                Icon = ""
            },

            new ClassSubjectOption
            {
                Name = "Teacher Remarks",
                Icon = ""
            },

            new ClassSubjectOption
            {
                Name = "Announcement",
                Icon = ""
            }
        };


        // =========================================================
        // PAGE LOAD
        // =========================================================

        public void OnGet()
        {
            // -----------------------------------------------------
            // TEMPORARY DATA
            //
            // Later this will come from the database.
            // -----------------------------------------------------

            Subjects = new List<ClassSubject>
            {
                new ClassSubject
                {
                    Name = "English",
                    TeacherName = "Mrs. Khan",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Mathematics",
                    TeacherName = "Mr. Ahmed",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Science",
                    TeacherName = "Ms. Fatima",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Pakistan Studies",
                    TeacherName = "Mr. Bilal",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Drawing",
                    TeacherName = "Mrs. Sana",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Art",
                    TeacherName = "Mr. Imran",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Social Studies",
                    TeacherName = "Mrs. Noreen",
                    TeacherImageUrl = ""
                },

                new ClassSubject
                {
                    Name = "Urdu",
                    TeacherName = "Mr. Yousuf",
                    TeacherImageUrl = ""
                }
            };
        }
    }


    // =============================================================
    // CLASS SUBJECT MODEL
    //
    // Named ClassSubject instead of Subject to avoid conflicts
    // with other Subject classes in the LMS project.
    //
    // Later this can be replaced with/mapped to the database model.
    // =============================================================

    public class ClassSubject
    {
        public string Name { get; set; } = "";

        public string TeacherName { get; set; } = "";

        public string TeacherImageUrl { get; set; } = "";
    }


    // =============================================================
    // SUBJECT OPTION MODEL
    // =============================================================

    public class ClassSubjectOption
    {
        public string Name { get; set; } = "";

        public string Icon { get; set; } = ""; 
    }
}