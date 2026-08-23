namespace Graber.Application.Errors;

public sealed record DownloadError(
    DownloadErrorCode Code) : Error();