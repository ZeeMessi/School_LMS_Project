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