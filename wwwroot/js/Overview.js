
document.addEventListener("DOMContentLoaded", function () {

    /*
     * ============================================================
     * SCHOOL LMS - OVERVIEW
     * ============================================================
     *
     * Current purpose:
     * - Handle fullscreen mode for selected overview cards.
     *
     * Future purpose:
     * - Database-driven updates can be handled by the server
     *   through Overview.cshtml.cs.
     */


    const fullscreenButtons =
        document.querySelectorAll(
            ".overview-fullscreen-button"
        );


    fullscreenButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            const targetId =
                button.getAttribute(
                    "data-fullscreen-target"
                );


            const target =
                document.getElementById(targetId);


            if (!target) {
                return;
            }


            const card =
                target.closest(".overview-card");


            if (!card) {
                return;
            }


            const alreadyFullscreen =
                card.classList.contains(
                    "is-fullscreen"
                );


            if (alreadyFullscreen) {

                card.classList.remove(
                    "is-fullscreen"
                );

                document.body.classList.remove(
                    "overview-fullscreen-active"
                );

                button.textContent = "⛶";

                button.setAttribute(
                    "aria-label",
                    "View full screen"
                );

            }
            else {

                card.classList.add(
                    "is-fullscreen"
                );

                document.body.classList.add(
                    "overview-fullscreen-active"
                );

                button.textContent = "⤢";

                button.setAttribute(
                    "aria-label",
                    "Exit full screen"
                );

            }

        });

    });


    /*
     * ESC KEY
     *
     * Allows the user to exit fullscreen mode using
     * the Escape key.
     */

    document.addEventListener(
        "keydown",
        function (event) {

            if (event.key !== "Escape") {
                return;
            }


            const fullscreenCard =
                document.querySelector(
                    ".overview-card.is-fullscreen"
                );


            if (!fullscreenCard) {
                return;
            }


            fullscreenCard.classList.remove(
                "is-fullscreen"
            );

            document.body.classList.remove(
                "overview-fullscreen-active"
            );


            const button =
                fullscreenCard.querySelector(
                    ".overview-fullscreen-button"
                );


            if (button) {

                button.textContent = "⛶";

                button.setAttribute(
                    "aria-label",
                    "View full screen"
                );

            }

        }
    );

});
