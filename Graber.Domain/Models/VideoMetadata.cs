namespace Graber.Domain.Models;

public record VideoMetadata(
    string FileName,
    string Extension,
    string MimeType,
    double Duration,
    double Width,
    double Height
    );