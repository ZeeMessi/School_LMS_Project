/* ============================================================
   SCHOOL LMS - PROGRESS PAGE JAVASCRIPT
   ============================================================ */


/* ============================================================
   WAIT FOR PAGE
   ============================================================ */

document.addEventListener("DOMContentLoaded", function () {

    /* ========================================================
       PERFORMANCE TREND CHART
       ======================================================== */

    const performanceCanvas =
        document.getElementById("performanceTrendChart");

    if (performanceCanvas) {

        new Chart(performanceCanvas, {

            type: "line",

            data: {

                labels: [
                    "January",
                    "February",
                    "March",
                    "April",
                    "May",
                    "June",
                    "July",
                    "August"
                ],

                datasets: [

                    {
                        label: "Overall Performance",

                        data: [
                            78,
                            79,
                            82,
                            81,
                            85,
                            84,
                            86,
                            87
                        ],

                        borderWidth: 3,

                        tension: 0.4,

                        fill: true,

                        pointRadius: 5,

                        pointHoverRadius: 8
                    }

                ]

            },

            options: {

                responsive: true,

                maintainAspectRatio: false,

                interaction: {
                    mode: "index",
                    intersect: false
                },

                plugins: {

                    legend: {
                        display: false
                    },

                    tooltip: {

                        callbacks: {

                            label: function (context) {

                                return " Performance: "
                                    + context.parsed.y
                                    + "%";

                            }

                        }

                    }

                },

                scales: {

                    y: {

                        min: 0,

                        max: 100,

                        ticks: {

                            callback: function (value) {

                                return value + "%";

                            }

                        }

                    }

                }

            }

        });

    }


    /* ========================================================
       ASSESSMENT BREAKDOWN CHART
       ======================================================== */

    const assessmentCanvas =
        document.getElementById("assessmentChart");

    if (assessmentCanvas) {

        new Chart(assessmentCanvas, {

            type: "doughnut",

            data: {

                labels: [
                    "Monthly Tests",
                    "Quizzes",
                    "Assignments",
                    "Quarterly Exam",
                    "Annual Exam"
                ],

                datasets: [

                    {

                        data: [
                            84,
                            91,
                            95,
                            87,
                            89
                        ],

                        borderWidth: 3,

                        hoverOffset: 12

                    }

                ]

            },

            options: {

                responsive: true,

                maintainAspectRatio: false,

                cutout: "62%",

                plugins: {

                    legend: {

                        position: "bottom",

                        labels: {

                            padding: 16,

                            usePointStyle: true,

                            font: {
                                size: 11
                            }

                        }

                    },

                    tooltip: {

                        callbacks: {

                            label: function (context) {

                                return " "
                                    + context.label
                                    + ": "
                                    + context.parsed
                                    + "%";

                            }

                        }

                    }

                }

            }

        });

    }


    /* ========================================================
       ATTENDANCE VS PERFORMANCE
       ======================================================== */

    const attendanceCanvas =
        document.getElementById(
            "attendancePerformanceChart"
        );

    if (attendanceCanvas) {

        new Chart(attendanceCanvas, {

            type: "line",

            data: {

                labels: [
                    "January",
                    "February",
                    "March",
                    "April",
                    "May",
                    "June",
                    "July",
                    "August"
                ],

                datasets: [

                    {

                        label: "Attendance",

                        data: [
                            91,
                            93,
                            92,
                            95,
                            94,
                            96,
                            95,
                            94
                        ],

                        borderWidth: 3,

                        tension: 0.4,

                        pointRadius: 4,

                        pointHoverRadius: 7

                    },

                    {

                        label: "Academic Performance",

                        data: [
                            78,
                            79,
                            82,
                            81,
                            85,
                            84,
                            86,
                            87
                        ],

                        borderWidth: 3,

                        tension: 0.4,

                        pointRadius: 4,

                        pointHoverRadius: 7

                    }

                ]

            },

            options: {

                responsive: true,

                maintainAspectRatio: false,

                interaction: {

                    mode: "index",

                    intersect: false

                },

                plugins: {

                    legend: {

                        position: "bottom",

                        labels: {

                            usePointStyle: true,

                            padding: 15

                        }

                    },

                    tooltip: {

                        callbacks: {

                            label: function (context) {

                                return " "
                                    + context.dataset.label
                                    + ": "
                                    + context.parsed.y
                                    + "%";

                            }

                        }

                    }

                },

                scales: {

                    y: {

                        min: 50,

                        max: 100,

                        ticks: {

                            callback: function (value) {

                                return value + "%";

                            }

                        }

                    }

                }

            }

        });

    }


    /* ========================================================
       FILTERS
       ======================================================== */

    const academicYear =
        document.getElementById("academicYear");

    const progressTerm =
        document.getElementById("progressTerm");

    const progressSubject =
        document.getElementById("progressSubject");

    const progressPeriod =
        document.getElementById("progressPeriod");


    function handleProgressFilterChange() {

        /*
         * Future database integration:
         *
         * The selected values will eventually be sent
         * to the server/database.
         *
         * Example:
         *
         * academicYear.value
         * progressTerm.value
         * progressSubject.value
         * progressPeriod.value
         *
         * The server will then return the appropriate
         * student progress information.
         */

        console.log(
            "Progress filters changed:",
            {
                academicYear:
                    academicYear
                        ? academicYear.value
                        : null,

                term:
                    progressTerm
                        ? progressTerm.value
                        : null,

                subject:
                    progressSubject
                        ? progressSubject.value
                        : null,

                period:
                    progressPeriod
                        ? progressPeriod.value
                        : null
            }
        );

    }


    if (academicYear) {
        academicYear.addEventListener(
            "change",
            handleProgressFilterChange
        );
    }


    if (progressTerm) {
        progressTerm.addEventListener(
            "change",
            handleProgressFilterChange
        );
    }


    if (progressSubject) {
        progressSubject.addEventListener(
            "change",
            handleProgressFilterChange
        );
    }


    if (progressPeriod) {
        progressPeriod.addEventListener(
            "change",
            handleProgressFilterChange
        );

    }

});