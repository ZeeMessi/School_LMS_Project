/* ============================================================
   SCHOOL LMS - GRADE PAGE JAVASCRIPT
   ============================================================ */

document.addEventListener("DOMContentLoaded", function () {

    /* ============================================================
       DATABASE-READY DEMO DATA

       Later these values will come from the database/API.

       Do not build the final system around these hard-coded values.
       ============================================================ */

    const examinationData = {

        monthly: {
            subjects: [
                { name: "English", marks: 82, total: 100, grade: "A" },
                { name: "Maths", marks: 85, total: 100, grade: "A" },
                { name: "Science", marks: 78, total: 100, grade: "B+" },
                { name: "Urdu", marks: 74, total: 100, grade: "B" },
                { name: "Drawing", marks: 90, total: 100, grade: "A+" }
            ]
        },

        quarterly: {
            subjects: [
                { name: "English", marks: 86, total: 100, grade: "A" },
                { name: "Maths", marks: 89, total: 100, grade: "A" },
                { name: "Science", marks: 80, total: 100, grade: "A" },
                { name: "Urdu", marks: 75, total: 100, grade: "B+" },
                { name: "Drawing", marks: 92, total: 100, grade: "A+" }
            ]
        },

        midterm: {
            subjects: [
                { name: "English", marks: 88, total: 100, grade: "A" },
                { name: "Maths", marks: 91, total: 100, grade: "A+" },
                { name: "Science", marks: 84, total: 100, grade: "A" },
                { name: "Urdu", marks: 77, total: 100, grade: "B+" },
                { name: "Drawing", marks: 94, total: 100, grade: "A+" }
            ]
        },

        annual: {
            subjects: [
                { name: "English", marks: 88, total: 100, grade: "A" },
                { name: "Maths", marks: 92, total: 100, grade: "A+" },
                { name: "Science", marks: 81, total: 100, grade: "A" },
                { name: "Urdu", marks: 76, total: 100, grade: "B+" },
                { name: "Drawing", marks: 90, total: 100, grade: "A+" }
            ]
        }

    };


    /* ============================================================
       ELEMENTS
       ============================================================ */

    const chart = document.getElementById("performanceChart");
    const tableBody = document.getElementById("gradeTableBody");

    const overallPercentage =
        document.getElementById("overallPercentage");

    const overallGrade =
        document.getElementById("overallGrade");

    const totalMarks =
        document.getElementById("totalMarks");

    const subjectsPassed =
        document.getElementById("subjectsPassed");


    /* ============================================================
       GRADE CALCULATION

       This is temporary.

       Later the grading rules should come from the database /
       school configuration.
       ============================================================ */

    function calculateGrade(percentage) {

        if (percentage >= 90) return "A+";
        if (percentage >= 80) return "A";
        if (percentage >= 75) return "B+";
        if (percentage >= 70) return "B";
        if (percentage >= 65) return "C+";
        if (percentage >= 50) return "C";

        return "F";
    }


    /* ============================================================
       GRADE BADGE CLASS
       ============================================================ */

    function getGradeClass(grade) {

        if (grade === "A+") {
            return "grade-aplus";
        }

        if (grade === "A") {
            return "grade-a";
        }

        if (grade === "B+") {
            return "grade-bplus";
        }

        return "";
    }


    /* ============================================================
       RENDER PERFORMANCE CHART
       ============================================================ */

    function renderPerformanceChart(subjects) {

        if (!chart) return;

        chart.innerHTML = "";

        subjects.forEach(function (subject) {

            const percentage =
                (subject.marks / subject.total) * 100;

            const column =
                document.createElement("div");

            column.className = "chart-column";

            const bar =
                document.createElement("div");

            bar.className = "chart-bar";

            bar.style.setProperty(
                "--bar-height",
                `${percentage}%`
            );

            const percentageText =
                document.createElement("span");

            percentageText.className =
                "chart-percentage";

            percentageText.textContent =
                `${Math.round(percentage)}%`;

            const subjectText =
                document.createElement("span");

            subjectText.className =
                "chart-subject";

            subjectText.textContent =
                subject.name;

            const tooltip =
                document.createElement("div");

            tooltip.className =
                "chart-tooltip";

            tooltip.innerHTML = `
                <strong>${subject.name}</strong><br>
                Marks: ${subject.marks}/${subject.total}<br>
                Percentage: ${Math.round(percentage)}%<br>
                Grade: ${subject.grade}
            `;

            bar.appendChild(percentageText);
            bar.appendChild(tooltip);

            column.appendChild(bar);
            column.appendChild(subjectText);

            chart.appendChild(column);

        });

    }


    /* ============================================================
       RENDER TABLE
       ============================================================ */

    function renderTable(subjects) {

        if (!tableBody) return;

        tableBody.innerHTML = "";

        subjects.forEach(function (subject) {

            const percentage =
                Math.round(
                    (subject.marks / subject.total) * 100
                );

            const row =
                document.createElement("tr");

            row.innerHTML = `
                <td>${subject.name}</td>
                <td>${subject.total}</td>
                <td>${subject.marks}</td>
                <td>${percentage}%</td>
                <td>
                    <span class="grade-badge ${getGradeClass(subject.grade)}">
                        ${subject.grade}
                    </span>
                </td>
                <td>
                    ${percentage >= 90
                        ? "Excellent"
                        : percentage >= 80
                            ? "Very Good"
                            : percentage >= 70
                                ? "Good"
                                : "Needs Improvement"}
                </td>
            `;

            tableBody.appendChild(row);

        });

    }


    /* ============================================================
       UPDATE SUMMARY
       ============================================================ */

    function updateSummary(subjects) {

        let obtained = 0;
        let total = 0;
        let passed = 0;

        subjects.forEach(function (subject) {

            obtained += subject.marks;
            total += subject.total;

            const percentage =
                (subject.marks / subject.total) * 100;

            if (percentage >= 50) {
                passed++;
            }

        });

        const percentage =
            total > 0
                ? (obtained / total) * 100
                : 0;

        const grade =
            calculateGrade(percentage);

        if (overallPercentage) {
            overallPercentage.textContent =
                `${percentage.toFixed(1)}%`;
        }

        if (overallGrade) {
            overallGrade.textContent =
                grade;
        }

        if (totalMarks) {
            totalMarks.textContent =
                `${obtained} / ${total}`;
        }

        if (subjectsPassed) {
            subjectsPassed.textContent =
                `${passed} / ${subjects.length}`;
        }

    }


    /* ============================================================
       LOAD EXAMINATION
       ============================================================ */

    function loadExamination(examination) {

        const data =
            examinationData[examination];

        if (!data) return;

        renderPerformanceChart(data.subjects);

        renderTable(data.subjects);

        updateSummary(data.subjects);

    }


    /* ============================================================
       EXAMINATION BUTTONS
       ============================================================ */

    const examButtons =
        document.querySelectorAll(".exam-button");

    examButtons.forEach(function (button) {

        button.addEventListener("click", function () {

            examButtons.forEach(function (item) {
                item.classList.remove("active");
            });

            button.classList.add("active");

            const examination =
                button.dataset.exam;

            loadExamination(examination);

        });

    });


    /* ============================================================
       PERFORMANCE HISTORY
       ============================================================ */

    const historyChart =
        document.getElementById("historyChart");

    const historyData = [
        { name: "Monthly", value: 82 },
        { name: "Quarterly", value: 86 },
        { name: "Mid-Term", value: 88 },
        { name: "Annual", value: 87.5 }
    ];

    if (historyChart) {

        historyData.forEach(function (item) {

            const column =
                document.createElement("div");

            column.className =
                "history-column";

            const point =
                document.createElement("div");

            point.className =
                "history-point";

            point.title =
                `${item.name}: ${item.value}%`;

            const value =
                document.createElement("span");

            value.className =
                "history-value";

            value.textContent =
                `${item.value}%`;

            const label =
                document.createElement("span");

            label.className =
                "history-label";

            label.textContent =
                item.name;

            point.appendChild(value);

            column.appendChild(point);
            column.appendChild(label);

            historyChart.appendChild(column);

        });

    }


    /* ============================================================
       INITIAL LOAD
       ============================================================ */

    loadExamination("monthly");

});