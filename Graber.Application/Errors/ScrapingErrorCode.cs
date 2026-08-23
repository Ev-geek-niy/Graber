namespace Graber.Application.Errors;

public enum ScrapingErrorCode
{
    MediaNotFound = 1,
    MediaPrivate,
    MediaRemoved,
    SourceUnavailable,
    MediaDiscoveryFailed
}