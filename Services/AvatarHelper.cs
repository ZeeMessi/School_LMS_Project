using SchoolLMS.Data.Entities;

namespace SchoolLMS.Services;

// A placeholder avatar for a teacher who hasn't uploaded a photo yet - a
// generic person silhouette tinted by gender, so browsing a list of
// teachers still gives a visual hint at a glance without pretending to be
// an actual photo of anyone. Returned as a data: URI so it can go straight
// into an <img src="..."> exactly like a real uploaded photo would,
// without a network round-trip or an extra file on disk.
public static class AvatarHelper
{
    public static string PlaceholderDataUri(Gender gender)
    {
        var background = gender == Gender.Female ? "#db2777" : "#2563eb";

        var svg =
            $"""
            <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100">
                <rect width="100" height="100" rx="14" fill="{background}"/>
                <circle cx="50" cy="38" r="18" fill="#ffffff" fill-opacity="0.92"/>
                <path d="M50 60c-23 0-36 13-36 28v6h72v-6c0-15-13-28-36-28z" fill="#ffffff" fill-opacity="0.92"/>
            </svg>
            """;

        var bytes = System.Text.Encoding.UTF8.GetBytes(svg);
        return "data:image/svg+xml;base64," + System.Convert.ToBase64String(bytes);
    }
}
