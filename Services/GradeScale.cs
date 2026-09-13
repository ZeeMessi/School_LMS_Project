namespace SchoolLMS.Services;

// Shared so every page that turns a percentage into a letter grade uses the
// same scale — matches the table shown on Pages/Grade.cshtml's "Grade
// Scale" section. A school with a different scale would change it here
// once, rather than in every page that computes a grade.
public static class GradeScale
{
    public static string LetterFor(double percentage) => percentage switch
    {
        >= 90 => "A+",
        >= 80 => "A",
        >= 75 => "B+",
        >= 70 => "B",
        >= 65 => "C+",
        >= 50 => "C",
        _ => "F"
    };
}
