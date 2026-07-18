using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.Application.Providers;

public class ScraperProvider(
    IEnumerable<IScraper<string, Video>> scrapers)
{
    public IScraper<string, Video>? GetScraper(string url)
    {
        return scrapers.FirstOrDefault(scraper => scraper.CanExecute(url));
    }
}