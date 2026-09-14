using Microsoft.AspNetCore.Http;

namespace SchoolLMS.Services;

// Shared by every Admin upload form (School logo, Student photo, Teacher
// photo) so the size/type rules live in exactly one place.
public static class ImageUploadHelper
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/png", "image/webp", "image/gif"
    };

    private const long MaxFileSizeBytes = 2 * 1024 * 1024; // 2 MB

    public static async Task<ImageUploadResult> ReadAsync(IFormFile? file)
    {
        if (file is null || file.Length == 0)
        {
            // Nothing chosen - not an error, just means "leave the
            // existing photo/logo alone" on an edit form.
            return new ImageUploadResult(null, null, null);
        }

        if (file.Length > MaxFileSizeBytes)
        {
            return new ImageUploadResult(null, null, "Image must be 2 MB or smaller.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            return new ImageUploadResult(null, null, "Only JPEG, PNG, WebP, or GIF images are allowed.");
        }

        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);
        return new ImageUploadResult(stream.ToArray(), file.ContentType, null);
    }
}

public record ImageUploadResult(byte[]? Data, string? ContentType, string? Error);
