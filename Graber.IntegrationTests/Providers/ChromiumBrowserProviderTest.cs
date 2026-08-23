using Graber.Infrastructure.Providers;
using Graber.Infrastructure.Scrapers;
using Microsoft.Extensions.Options;

namespace Graber.IntegrationTests.Providers;

public class ChromiumBrowserProviderTest : IAsyncLifetime
{
    private static readonly IOptions<XScraperOptions> options = Options.Create(new XScraperOptions()
    {
        Headless = false,
        PlaylistDiscoveryTimeout = TimeSpan.FromSeconds(10)
    });
    private readonly ChromiumBrowserProvider browserProvider = new(options);
    
    [Fact]
    public async Task GetBrowserAsync_WhenCalledConcurrently_ReturnsSameBrowser()
    {
        var firstBrowser = browserProvider.GetBrowserAsync(CancellationToken.None);
        var secondBrowser = browserProvider.GetBrowserAsync(CancellationToken.None);

        var browsers = await Task.WhenAll(firstBrowser, secondBrowser);
        
        Assert.Same(browsers[0], browsers[1]);
    }
    
    [Fact]
    public async Task DisposeAsync_WhenBrowserCreated_ClosesBrowser()
    {
        var browser = await browserProvider.GetBrowserAsync(CancellationToken.None);
        
        await browserProvider.DisposeAsync();
        
        Assert.True(browser.IsClosed);
    }

    [Fact]
    public async Task GetBrowserAsync_AfterProviderDisposed_Throws()
    {
        await browserProvider.DisposeAsync();
        
        await Assert.ThrowsAsync<ObjectDisposedException>(
            () => browserProvider.GetBrowserAsync(CancellationToken.None));
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public async Task DisposeAsync()
    {
        await browserProvider.DisposeAsync();
    }
}