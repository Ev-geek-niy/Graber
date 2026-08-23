using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Infrastructure.Providers;
using Microsoft.Extensions.Options;
using PuppeteerSharp;

namespace Graber.Infrastructure.Scrapers;

public class XScraper(
    IOptions<XScraperOptions> options,
    ChromiumBrowserProvider browserProvider) : IScraper
{
    private readonly XScraperOptions settings = options.Value;
    public bool CanExecute(string input)
    {
        return input.Contains("x.com");
    }

    public async Task<Result<string>> ExecuteAsync(string input, CancellationToken ct)
    {
        var playlistUrl = await GetPlaylistUrlAsync(input, ct);

        return playlistUrl.IsFailure 
            ? Result<string>.Failure(playlistUrl.Error) 
            : Result<string>.Success(playlistUrl.Value);
    }

    public async Task<Result<string>> GetPlaylistUrlAsync(string input, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        
        try
        {
            var browser = await browserProvider.GetBrowserAsync(ct);
            await using var page = await browser.NewPageAsync();
            var userAgent = await browser.GetUserAgentAsync();
            await page.SetUserAgentAsync(userAgent.Replace("Headless", ""));

            var playlistTask = page.WaitForResponseAsync(response =>
                response.Url.Contains(".m3u8", StringComparison.OrdinalIgnoreCase));

            await page.GoToAsync(input, new NavigationOptions()
            {
                WaitUntil = [WaitUntilNavigation.DOMContentLoaded]
            }).WaitAsync(ct);

            var response = await playlistTask.WaitAsync(settings.PlaylistDiscoveryTimeout, ct);
            var playlistUrl = response.Url;

            return string.IsNullOrEmpty(playlistUrl) 
                ? Result<string>.Failure(new ScrapingError(ScrapingErrorCode.MediaNotFound)) 
                : Result<string>.Success(playlistUrl);
        }
        catch (TimeoutException)
        {
            return Result<string>.Failure(new ScrapingError(ScrapingErrorCode.MediaNotFound));
        }
        catch (NavigationException)
        {
            return Result<string>.Failure(new ScrapingError(ScrapingErrorCode.MediaDiscoveryFailed));
        }
    }
}