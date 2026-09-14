using Microsoft.AspNetCore.Http;

namespace SchoolLMS.Services;

// Same idea as ImageUploadHelper, for the file a teacher attaches to an
// assignment/handout/quiz/announcement - a bigger size allowance and a
// wider set of file types than a profile photo, since these are meant to
// be real documents (PDFs, Word/Excel files, images of handwritten
// handouts, etc.), not just a headshot.
public static class DocumentUploadHelper
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "application/vnd.ms-excel",
        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        "application/vnd.ms-powerpoint",
        "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        "text/plain",
        "image/jpeg",
        "image/png",
        "image/webp",
        "image/gif"
    };

    private const long MaxFileSizeBytes = 10 * 1024 * 1024; // 10 MB

    public static async Task<DocumentUploadResult> ReadAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            // Nothing chosen - not an error, a material/announcement can
            // exist with no attachment at all.
            return new DocumentUploadResult(null, null, null, null);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return new DocumentUploadResult(null, null, null, "File must be 10 MB or smaller.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return new DocumentUploadResult(null, null, null, "Only PDF, Word, Excel, PowerPoint, plain text, or image files are allowed.");
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        return new DocumentUploadResult(stream.ToArray(), file.ContentType, file.FileName, null);
    }
}

public record DocumentUploadResult(byte[]? Data, string? ContentType, string? FileName, string? Error);
