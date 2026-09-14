/* ============================================================
   SCHOOL LMS - PROGRESS PAGE JAVASCRIPT
   ============================================================

   Both charts below are built from the real data the server
   rendered into the #progress-data JSON island (see Progress.cshtml)
   - there are no hardcoded numbers left in this file. The trend
   line and the assessment doughnut intentionally read the SAME
   "trend" data: they're two views of the same real exam sittings,
   not two separately-maintained datasets that could disagree.
   ============================================================ */

document.addEventListener("DOMContentLoaded", function () {

    const dataElement = document.getElementById("progress-data");

    if (!dataElement) {
        return; // no data island means the empty-state markup is showing instead
    }

    const progressData = JSON.parse(dataElement.textContent);


    /* ========================================================
       PERFORMANCE TREND CHART
       ======================================================== */

    const performanceCanvas =
        document.getElementById("performanceTrendChart");

    if (performanceCanvas && progressData.trend.labels.length > 0) {

        new Chart(performanceCanvas, {

            type: "line",

            data: {

                labels: progressData.trend.labels,

                datasets: [

                    {
                        label: "Overall Performance",

                        data: progressData.trend.values,

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

       Same data as the trend chart above - grouped by examination
       type instead of plotted over time.
       ======================================================== */

    const assessmentCanvas =
        document.getElementById("assessmentChart");

    if (assessmentCanvas && progressData.trend.labels.length > 0) {

        new Chart(assessmentCanvas, {

            type: "doughnut",

            data: {

                labels: progressData.trend.labels,

                datasets: [

                    {

                        data: progressData.trend.values,

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

       Null values (a month with attendance but no exam, or vice
       versa) render as gaps in the line - Chart.js handles that
       natively, so a sparse real history still renders correctly
       instead of needing to be padded with fake numbers.
       ======================================================== */

    const attendanceCanvas =
        document.getElementById(
            "attendancePerformanceChart"
        );

    const avp = progressData.attendanceVsPerformance;

    if (attendanceCanvas && avp.labels.length > 0) {

        new Chart(attendanceCanvas, {

            type: "line",

            data: {

                labels: avp.labels,

                datasets: [

                    {

                        label: "Attendance",

                        data: avp.attendance,

                        spanGaps: true,

                        borderWidth: 3,

                        tension: 0.4,

                        pointRadius: 4,

                        pointHoverRadius: 7

                    },

                    {

                        label: "Academic Performance",

                        data: avp.academic,

                        spanGaps: true,

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

                                if (context.parsed.y === null) {
                                    return "";
                                }

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

});
