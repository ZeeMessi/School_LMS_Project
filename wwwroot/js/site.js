/* ============================================================
   SCHOOL LMS - SHARED LAYOUT JAVASCRIPT
   ============================================================ */


/*
    SIDEBAR STATES

    OPEN:
        Logo       = visible
        Hamburger  = visible
        Icons      = visible
        Text       = visible

    CLOSED:
        Logo       = hidden
        Hamburger  = visible
        Icons      = visible
        Text       = hidden
*/


const sidebarToggle =
    document.getElementById("sidebarToggle");


if (sidebarToggle) {

    sidebarToggle.addEventListener("click", function () {

        document.body.classList.toggle(
            "sidebar-collapsed"
        );


        const sidebarIsCollapsed =
            document.body.classList.contains(
                "sidebar-collapsed"
            );


        /*
            Accessibility information.
        */

        sidebarToggle.setAttribute(
            "aria-expanded",
            (!sidebarIsCollapsed).toString()
        );

    });

}


/*
    NOTIFICATION BELL DROPDOWN

    Toggled on click; closes on an outside click or Escape, same
    pattern as any simple dropdown.
*/

const notificationButton =
    document.getElementById("notificationButton");

const notificationPanel =
    document.getElementById("notificationPanel");


if (notificationButton && notificationPanel) {

    notificationButton.addEventListener("click", function (event) {

        event.stopPropagation();

        const isHidden = notificationPanel.hidden;

        notificationPanel.hidden = !isHidden;

        notificationButton.setAttribute(
            "aria-expanded",
            isHidden.toString()
        );

    });


    document.addEventListener("click", function (event) {

        if (notificationPanel.hidden) {
            return;
        }

        const clickedInsidePanel =
            notificationPanel.contains(event.target) ||
            notificationButton.contains(event.target);

        if (!clickedInsidePanel) {
            notificationPanel.hidden = true;
            notificationButton.setAttribute("aria-expanded", "false");
        }

    });


    document.addEventListener("keydown", function (event) {

        if (event.key === "Escape" && !notificationPanel.hidden) {
            notificationPanel.hidden = true;
            notificationButton.setAttribute("aria-expanded", "false");
        }

    });

}