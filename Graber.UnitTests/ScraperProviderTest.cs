using Graber.Application.Providers;
using Graber.UnitTests.Stubs;

namespace Graber.UnitTests;

public class ScraperProviderTest
{
    [Fact]
    public void ScraperProvider_ReturnNull_ForUnsupportedUrl()
    {
        var stub = new StubScraper(false);
        var provider = new ScraperProvider([stub]);
        
        var nullScraper = provider.GetScraper("unsupportedUrl");
        Assert.Null(nullScraper);
    }

    [Fact]
    public void ScraperProvider_ReturnScraper_ForSupportedScraper()
    {
        var stub1 = new StubScraper(false);
        var stub2 = new StubScraper(true);
        var stub3 = new StubScraper(false);
        var provider = new ScraperProvider([stub1, stub2, stub3]);

        var targetScraper = provider.GetScraper("validUrl");
        
        Assert.Same(targetScraper, stub2);
    }
}