using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Application.Providers;

namespace Graber.Application.UseCases;

public class ProcessUrlUseCase(
    ScraperProvider scraperProvider,
    IResultPublisher publisher
    )
{
    public async Task ExecuteAsync(string url)
    {
        var scraper = scraperProvider.GetScraper(url);
        if (scraper == null)
        {
            await publisher.PublishAsync(Result.Failure(ScrapingErrorType.ServiceNotSupported));
            return;
        }

        var result = await scraper.ExecuteAsync(url);
        
        await publisher.PublishAsync(result);
    }
}