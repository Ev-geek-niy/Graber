using Graber.Infrastructure.Scrapers;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

namespace Graber.Infrastructure.Providers;

public sealed class ChromiumBrowserProvider(
    IOptions<XScraperOptions>  options) : IAsyncDisposable
{
    private readonly SemaphoreSlim _initializationLock = new(1, 1);
    private readonly XScraperOptions _settings = options.Value;
    
    private IBrowser? browser;
    private int disposeRequested;

    public async Task<IBrowser> GetBrowserAsync(CancellationToken cancellationToken)
    {
        ThrowIfDisposed();
        
        await _initializationLock.WaitAsync(cancellationToken);
        try
        {
            ThrowIfDisposed();
            if (browser is {IsClosed: false, IsConnected: true})
                return browser;

            if (browser is not null)
            {
                await browser.DisposeAsync();
                browser = null;
            }
            
            cancellationToken.ThrowIfCancellationRequested();

            if (_settings.BrowserExecutablePath is not null)
            {
                browser = await Puppeteer.LaunchAsync(new LaunchOptions()
                {
                    Headless = _settings.Headless,
                    ExecutablePath = _settings.BrowserExecutablePath
                });
            }
            else
            {
                await new BrowserFetcher().DownloadAsync();
                cancellationToken.ThrowIfCancellationRequested();
                browser = await Puppeteer.LaunchAsync(new LaunchOptions
                {
                    Headless = _settings.Headless,
                });
            }
            
            return browser;
        }
        finally
        {
            _initializationLock.Release();
        }
    }
    
    
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref disposeRequested, 1) != 0)
            return;
        
        await _initializationLock.WaitAsync();
        try
        {
            if (browser is not null)
            {
                await browser.DisposeAsync();
                browser = null;
            }
        }
        finally
        {
            _initializationLock.Release();
        }
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(
            Volatile.Read(ref disposeRequested) != 0, 
            this);
    }
}