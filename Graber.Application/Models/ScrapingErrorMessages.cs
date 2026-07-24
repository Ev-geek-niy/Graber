using Graber.Application.Enums;

namespace Graber.Application.Models;

public static class ScrapingErrorMessages
{
    private static readonly Dictionary<ScrapingErrorType, string> Messages = new()
    {
        { ScrapingErrorType.ServiceNotSupported, "The service is not supported." },
        { ScrapingErrorType.DeleteVideo, "The video is deleted." },
        { ScrapingErrorType.PrivateVideo, "The video is private." },
        { ScrapingErrorType.NotFoundVideo, "The video is not found." },
    };

    public static string GetMessage(ScrapingErrorType type)
    {
        return Messages.GetValueOrDefault(type, "Not typed error message.");
    }
}