using Graber.Application.Interfaces;

namespace Graber.Application.Providers;

public class ScraperProvider(
    IEnumerable<IScraper> scrapers)
{
    public IScraper? GetScraper(string url) => scrapers.FirstOrDefault(scraper => scraper.CanExecute(url));
}