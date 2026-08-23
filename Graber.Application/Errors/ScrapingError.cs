namespace Graber.Application.Errors;

public sealed record ScrapingError(
    ScrapingErrorCode Code) : Error;
