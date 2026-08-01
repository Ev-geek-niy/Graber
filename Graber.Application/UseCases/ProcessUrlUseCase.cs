using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Application.Providers;

namespace Graber.Application.UseCases;

public class ProcessUrlUseCase(
    ScraperProvider scraperProvider
    )
{
    public async Task<Result<Video>> ExecuteAsync(string url)
    {
        var scraper = scraperProvider.GetScraper(url);
        if (scraper == null)
            return Result.Failure(ScrapingErrorType.ServiceNotSupported);

        var result = await scraper.ExecuteAsync(url);
        if (result.IsFailure)
            return Result.Failure(result.Error);

        return result;
    }
}