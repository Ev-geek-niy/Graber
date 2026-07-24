using Graber.Application.Enums;
using Graber.Infrastructure.Scrapers;

namespace Graber.IntegrationTests.Scrapers;

public class XScraperTest
{
    [Fact]
    public async Task XScraper_GetPlaylistUrl_SuccessResult()
    {
        var xScraper = new XScraper();
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/2080134676878967139?s=20");
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task XScraper_GetPlaylistUrl_NotFoundResult()
    {
        var xScraper = new XScraper();
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/392912");
        
        if (result.Error == null)
            Assert.Fail();
        
        Assert.True(result.IsFailure && result.Error.Type == ScrapingErrorType.NotFoundVideo);
    }
}