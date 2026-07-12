using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.Application.Providers;

public class ScrapperProvider(
    IEnumerable<IScrapper<string, Video>> _scrappers)
{
    public Result<Video> Scrape(string url)
    {
        var handler = _scrappers.FirstOrDefault(scrapper => scrapper.CanExecute(url));
        return handler?.Execute(url) ?? throw new InvalidOperationException($"No handler found for this site: {url}");
    }
}