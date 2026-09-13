/* ============================================================
   SCHOOL LMS - CLASS PAGE JAVASCRIPT
   ============================================================

   This file intentionally contains only the Class-page behavior.

   Subject information, teacher information, images, class,
   section and permissions will eventually come from the
   database/backend.

   The visual structure does NOT depend on the temporary data.
   ============================================================ */


/* ============================================================
   SUBJECT OPTION PLACEHOLDER

   Later each option can navigate to its actual page.

   Examples:
   Assignment -> /Assignments
   Quiz       -> /Quizzes
   Handouts   -> /Handouts
   Remarks    -> /TeacherRemarks
   Announcement -> /Announcements

   No actual navigation is implemented yet.
   ============================================================ */

document.querySelectorAll(".subject-option").forEach(option => {

    option.addEventListener("click", function (event) {

        event.preventDefault();

        const optionName =
            this.dataset.option;

        console.log(
            "Selected subject option:",
            optionName
        );

    });

});