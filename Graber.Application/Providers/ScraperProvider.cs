using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.Application.Providers;

public class ScraperProvider(
    IEnumerable<Iscraper<string, Video>> scrapers)
{
    public Result<Iscraper<string, Video>> GetScraper(string url)
    {
        var handler = scrapers.FirstOrDefault(scraper => scraper.CanExecute(url));
        return handler is not null
            ? Result.Success(handler)
            : Result.Failure(ScrapingErrorType.ServiceNotSupported);
    }
}