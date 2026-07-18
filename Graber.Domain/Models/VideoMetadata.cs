using Graber.Domain.Abstract;

namespace Graber.Domain.Models;

public record VideoMetadata(
    string FileName,
    string Extension,
    string? MimeType,
    double Duration,
    double Width,
    double Height
) : RecordWithValidation
{
    protected override void Validate()
    {
        if (string.IsNullOrEmpty(FileName))
            throw new ArgumentNullException($"{nameof(FileName)} cannot be null or empty.");
        if (string.IsNullOrEmpty(Extension))
            throw new ArgumentNullException($"{nameof(Extension)} cannot be null or empty.");
        if (Duration < 0)
            throw new ArgumentOutOfRangeException($"{nameof(Duration)} cannot be negative.");
        if (Width < 0)
            throw new ArgumentOutOfRangeException($"{nameof(Width)} cannot be negative.");
        if (Height < 0)
            throw new ArgumentOutOfRangeException($"{nameof(Height)} cannot be negative.");
    }
}