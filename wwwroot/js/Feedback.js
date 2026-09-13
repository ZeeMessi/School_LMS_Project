document.addEventListener("DOMContentLoaded", function () {

    /* ========================================================
       RATING
       ======================================================== */

    const ratingContainer =
        document.getElementById("feedbackRating");

    const ratingValue =
        document.getElementById("ratingValue");

    const ratingText =
        document.getElementById("ratingText");

    const stars =
        document.querySelectorAll(".rating-star");


    const ratingLabels = {
        1: "Very Poor",
        2: "Poor",
        3: "Average",
        4: "Good",
        5: "Excellent"
    };


    if (ratingContainer && ratingValue && ratingText) {

        stars.forEach(function (star) {

            star.addEventListener("click", function () {

                const selectedRating =
                    Number(this.dataset.rating);

                ratingValue.value =
                    selectedRating;


                stars.forEach(function (item) {

                    const itemRating =
                        Number(item.dataset.rating);

                    if (itemRating <= selectedRating) {

                        item.classList.add("selected");

                    } else {

                        item.classList.remove("selected");

                    }

                });


                ratingText.textContent =
                    ratingLabels[selectedRating];

            });

        });

    }


    /* ========================================================
       CHARACTER COUNTER
       ======================================================== */

    const message =
        document.getElementById("feedbackMessage");

    const characterCount =
        document.getElementById("characterCount");


    function updateCharacterCount() {

        if (!message || !characterCount) {
            return;
        }


        const currentLength =
            message.value.length;


        characterCount.textContent =
            currentLength + " / 1000";


        if (currentLength >= 900) {

            characterCount.style.fontWeight =
                "700";

        } else {

            characterCount.style.fontWeight =
                "400";

        }

    }


    if (message) {

        message.addEventListener(
            "input",
            updateCharacterCount
        );

        updateCharacterCount();

    }


    /* ========================================================
       FORM SUBMISSION
       ======================================================== */

    const form =
        document.getElementById("feedbackForm");

    const submitButton =
        document.getElementById("submitFeedback");


    if (form && submitButton) {

        form.addEventListener("submit", function () {

            /*
             * Razor Pages will handle the actual POST.
             *
             * This JavaScript only prevents accidental
             * double-click submissions.
             */

            if (form.checkValidity()) {

                submitButton.disabled = true;

                const buttonText =
                    submitButton.querySelector("span:last-child");

                if (buttonText) {

                    buttonText.textContent =
                        "Submitting...";
                }

            }

        });

    }

});