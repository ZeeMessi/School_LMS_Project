/* ============================================================
   SCHOOL LMS - ANNOUNCEMENT JAVASCRIPT
   ============================================================ */


/* ============================================================
   OPEN ANNOUNCEMENT

   Reads the announcement's details from the data-* attributes on the
   clicked card's title button, which the server already rendered from
   the real Announcement row - rather than looking the id up in a
   separate hardcoded dataset that could (and did) disagree with the
   card underneath it.
   ============================================================ */

function openAnnouncement(button) {

    const data = button.dataset;


    const modal =
        document.getElementById("announcementModal");

    const title =
        document.getElementById("detailTitle");

    const category =
        document.getElementById("detailCategory");

    const date =
        document.getElementById("detailDate");

    const issuer =
        document.getElementById("detailIssuer");

    const body =
        document.getElementById("detailBody");


    title.textContent =
        data.title;

    category.textContent =
        data.category;

    date.textContent =
        data.date;

    issuer.textContent =
        data.issuer;

    body.textContent =
        data.content;


    /* --------------------------------------------------------
       ATTACHMENT
       -------------------------------------------------------- */

    const attachmentBox =
        document.getElementById("detailAttachment");

    const attachmentLink =
        document.getElementById("attachmentLink");


    if (data.attachmentUrl) {

        attachmentBox.style.display = "flex";

        attachmentLink.href =
            data.attachmentUrl;

        attachmentLink.textContent =
            data.attachmentName || "View Attachment";

    }
    else {

        attachmentBox.style.display =
            "none";

        attachmentLink.href =
            "#";

    }


    /* --------------------------------------------------------
       SHOW MODAL
       -------------------------------------------------------- */

    modal.classList.add("show");

    modal.setAttribute(
        "aria-hidden",
        "false"
    );


    document.body.style.overflow =
        "hidden";
}


/* ============================================================
   CLOSE ANNOUNCEMENT
   ============================================================ */

function closeAnnouncement() {

    const modal =
        document.getElementById("announcementModal");


    modal.classList.remove("show");

    modal.classList.remove("fullscreen");

    modal.setAttribute(
        "aria-hidden",
        "true"
    );


    document.body.style.overflow =
        "";
}


/* ============================================================
   FULL SCREEN
   ============================================================ */

function toggleAnnouncementFullscreen() {

    const modal =
        document.getElementById("announcementModal");


    modal.classList.toggle("fullscreen");
}


/* ============================================================
   FILTERS
   ============================================================ */

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const filters =
            document.querySelectorAll(
                ".announcement-filter"
            );

        const cards =
            document.querySelectorAll(
                ".announcement-card"
            );


        filters.forEach(
            function (filter) {

                filter.addEventListener(
                    "click",
                    function () {

                        filters.forEach(
                            function (item) {

                                item.classList.remove(
                                    "active"
                                );

                            }
                        );


                        filter.classList.add(
                            "active"
                        );


                        const selectedFilter =
                            filter.dataset.filter;


                        cards.forEach(
                            function (card) {

                                const category =
                                    card.dataset.category;

                                const important =
                                    card.dataset.important;


                                let showCard = false;


                                if (
                                    selectedFilter ===
                                    "all"
                                ) {

                                    showCard = true;

                                }
                                else if (
                                    selectedFilter ===
                                    "important"
                                ) {

                                    showCard =
                                        important ===
                                        "true";

                                }
                                else {

                                    showCard =
                                        category ===
                                        selectedFilter;

                                }


                                card.style.display =
                                    showCard
                                        ? "flex"
                                        : "none";

                            }
                        );

                    }
                );

            }
        );

    }
);


/* ============================================================
   ESCAPE KEY
   ============================================================ */

document.addEventListener(
    "keydown",
    function (event) {

        if (
            event.key === "Escape"
        ) {

            closeAnnouncement();

        }

    }
);


/* ============================================================
   CLICK OUTSIDE MODAL
   ============================================================ */

document.addEventListener(
    "click",
    function (event) {

        const modal =
            document.getElementById(
                "announcementModal"
            );


        if (
            event.target === modal
        ) {

            closeAnnouncement();

        }

    }
);