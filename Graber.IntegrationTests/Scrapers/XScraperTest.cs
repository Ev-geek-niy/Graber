using Graber.Application.Enums;
using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Scrapers;

namespace Graber.IntegrationTests.Scrapers;

public class XScraperTest
{
    [Fact]
    public async Task XScraper_GetPlaylistUrl_SuccessResult()
    {
        var downloader = new FFMpegHlsDownloader();
        var xScraper = new XScraper(downloader);
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/2080134676878967139?s=20");
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task XScraper_GetPlaylistUrl_NotFoundResult()
    {
        var downloader = new FFMpegHlsDownloader();
        var xScraper = new XScraper(downloader);
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/392912");

        Assert.NotNull(result.Error);        
        Assert.True(result.IsFailure && result.Error.Type == ScrapingErrorType.NotFoundVideo);
    }
}