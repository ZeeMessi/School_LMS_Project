/* ============================================================
   SCHOOL LMS - ANNOUNCEMENT JAVASCRIPT
   ============================================================ */


/* ============================================================
   SAMPLE DATABASE DATA
   ============================================================ */

/*
 * Temporary front-end data.
 *
 * Later this information will come from the database/API.
 *
 * The UI does not need to change when the database is connected.
 */

const announcementData = {

    1: {
        title: "Parent-Teacher Meeting",

        category: "Notice",

        date: "August 13, 2026",

        issuer: "School Administration",

        content:
            "The school administration has announced a parent-teacher meeting. Parents will receive further details regarding the meeting schedule and venue.",

        attachment: null
    },


    2: {
        title: "Annual Examination Schedule",

        category: "Notice",

        date: "August 10, 2026",

        issuer: "Examination Department",

        content:
            "The annual examination schedule has been published by the school administration. Students should check the examination schedule carefully.",

        attachment: null
    },


    3: {
        title: "School Activity",

        category: "Event",

        date: "August 7, 2026",

        issuer: "School Administration",

        content:
            "Students are requested to participate in the scheduled school activity.",

        attachment: null
    },


    4: {
        title: "School Holiday Notice",

        category: "Notice",

        date: "August 3, 2026",

        issuer: "School Administration",

        content:
            "The school administration has announced a holiday. Regular classes will resume according to the school timetable.",

        attachment: null
    }

};


/* ============================================================
   OPEN ANNOUNCEMENT
   ============================================================ */

function openAnnouncement(button) {

    const announcementId = button.dataset.id;

    const announcement =
        announcementData[announcementId];

    if (!announcement) {
        return;
    }


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
        announcement.title;

    category.textContent =
        announcement.category;

    date.textContent =
        announcement.date;

    issuer.textContent =
        announcement.issuer;

    body.textContent =
        announcement.content;


    /* --------------------------------------------------------
       ATTACHMENT
       -------------------------------------------------------- */

    const attachmentBox =
        document.getElementById("detailAttachment");

    const attachmentLink =
        document.getElementById("attachmentLink");


    if (announcement.attachment) {

        attachmentBox.style.display = "flex";

        attachmentLink.href =
            announcement.attachment.url;

        attachmentLink.textContent =
            announcement.attachment.name;

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