/* ============================================================
   SCHOOL LMS - ACCOUNT BOOK JAVASCRIPT
   ============================================================ */

document.addEventListener("DOMContentLoaded", function () {

    const viewButton =
        document.getElementById("viewChallanButton");

    const downloadButton =
        document.getElementById("downloadChallanButton");

    const modal =
        document.getElementById("challanModal");

    const overlay =
        document.getElementById("challanOverlay");

    const closeButton =
        document.getElementById("closeChallanButton");

    const closeFooterButton =
        document.getElementById("closeChallanFooterButton");

    const printButton =
        document.getElementById("printChallanButton");


    /* ========================================================
       OPEN CHALLAN
       ======================================================== */

    function openChallan() {

        if (!modal) {
            return;
        }

        modal.classList.add("active");

        modal.setAttribute("aria-hidden", "false");

        document.body.style.overflow = "hidden";
    }


    /* ========================================================
       CLOSE CHALLAN
       ======================================================== */

    function closeChallan() {

        if (!modal) {
            return;
        }

        modal.classList.remove("active");

        modal.setAttribute("aria-hidden", "true");

        document.body.style.overflow = "";
    }


    /* ========================================================
       VIEW CHALLAN
       ======================================================== */

    if (viewButton) {

        viewButton.addEventListener(
            "click",
            function () {

                openChallan();

            }
        );

    }


    /* ========================================================
       DOWNLOAD CHALLAN
       
       Browser print dialog allows:
       - Print
       - Save as PDF
       - Select printer
       
       The actual PDF generation can later be replaced by
       server-side PDF generation from the database.
       ======================================================== */

    if (downloadButton) {

        downloadButton.addEventListener(
            "click",
            function () {

                openChallan();

                setTimeout(function () {

                    window.print();

                }, 250);

            }
        );

    }


    /* ========================================================
       PRINT / DOWNLOAD FROM CHALLAN
       ======================================================== */

    if (printButton) {

        printButton.addEventListener(
            "click",
            function () {

                window.print();

            }
        );

    }


    /* ========================================================
       CLOSE BUTTON
       ======================================================== */

    if (closeButton) {

        closeButton.addEventListener(
            "click",
            function () {

                closeChallan();

            }
        );

    }


    /* ========================================================
       FOOTER CLOSE BUTTON
       ======================================================== */

    if (closeFooterButton) {

        closeFooterButton.addEventListener(
            "click",
            function () {

                closeChallan();

            }
        );

    }


    /* ========================================================
       CLICK OUTSIDE CHALLAN
       ======================================================== */

    if (overlay) {

        overlay.addEventListener(
            "click",
            function () {

                closeChallan();

            }
        );

    }


    /* ========================================================
       ESC KEY
       ======================================================== */

    document.addEventListener(
        "keydown",
        function (event) {

            if (
                event.key === "Escape" &&
                modal &&
                modal.classList.contains("active")
            ) {

                closeChallan();

            }

        }
    );


    /* ========================================================
       RECEIPT BUTTONS
       ======================================================== */

    const receiptButtons =
        document.querySelectorAll(".receipt-button");


    receiptButtons.forEach(function (button) {

        button.addEventListener(
            "click",
            function () {

                const receiptNumber =
                    button.getAttribute("data-receipt");

                if (receiptNumber) {

                    alert(
                        "Receipt Number: " +
                        receiptNumber
                    );

                }

            }
        );

    });

});