using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.Application.Providers;

public class ScraperProvider(
    IEnumerable<IScraper> scrapers)
{
    public IScraper? GetScraper(string url) => scrapers.FirstOrDefault(scraper => scraper.CanExecute(url));
}