/* ============================================================
   SCHOOL LMS - EXAM SCHEDULE JS
   ============================================================ */

document.addEventListener("DOMContentLoaded", function () {

    /* ========================================================
       FULLSCREEN
       ======================================================== */

    const fullscreenCards =
        document.querySelectorAll(".fullscreen-card");


    fullscreenCards.forEach(function (card) {

        const button =
            card.querySelector(".fullscreen-button");


        if (!button) {
            return;
        }


        button.addEventListener("click", function () {

            if (!document.fullscreenElement) {

                if (card.requestFullscreen) {
                    card.requestFullscreen();
                }
                else if (card.webkitRequestFullscreen) {
                    card.webkitRequestFullscreen();
                }
                else if (card.msRequestFullscreen) {
                    card.msRequestFullscreen();
                }

            }
            else {

                if (document.exitFullscreen) {
                    document.exitFullscreen();
                }
            }

        });

    });


    /* ========================================================
       UPDATE FULLSCREEN ICON
       ======================================================== */

    document.addEventListener(
        "fullscreenchange",
        function () {

            fullscreenCards.forEach(function (card) {

                const button =
                    card.querySelector(".fullscreen-button");

                if (!button) {
                    return;
                }


                if (document.fullscreenElement === card) {

                    button.textContent = "⛶";

                    button.title =
                        "Exit full screen";

                    button.setAttribute(
                        "aria-label",
                        "Exit full screen"
                    );

                }
                else {

                    button.textContent = "⛶";

                    button.title =
                        "Enter full screen";

                    button.setAttribute(
                        "aria-label",
                        "Enter full screen"
                    );
                }

            });

        }
    );


    /* ========================================================
       EXAM TYPE BUTTONS
       ======================================================== */

    const examTypeButtons =
        document.querySelectorAll(".exam-type-button");


    examTypeButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            examTypeButtons.forEach(function (item) {

                item.classList.remove("active");

            });


            button.classList.add("active");


            /*
             * DATABASE INTEGRATION LATER
             *
             * The selected exam type can later be sent to
             * the server/database here.
             *
             * Example:
             *
             * const examType =
             *     button.dataset.examType;
             *
             * fetch(`/ExamSchedule?examType=${examType}`);
             */

        });

    });


    /* ========================================================
       CALENDAR NAVIGATION
       ======================================================== */

    const calendarNavigation =
        document.querySelectorAll(".calendar-navigation");


    calendarNavigation.forEach(function (button) {

        button.addEventListener("click", function () {

            /*
             * Calendar navigation is intentionally prepared
             * for database/server integration.
             *
             * The real calendar month will eventually be
             * loaded from the exam schedule data.
             */

            console.log(
                "Calendar navigation selected."
            );

        });

    });


    /* ========================================================
       ESC KEY
       Browser automatically exits fullscreen with ESC.
       This listener simply keeps the UI synchronized.
       ======================================================== */

    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key === "Escape") {

                /*
                 * Browser handles fullscreen exit.
                 * No additional action is required.
                 */

            }

        }
    );

});