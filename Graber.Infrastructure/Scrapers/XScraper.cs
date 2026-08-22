using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using PuppeteerSharp;
using ScrapingError = Graber.Application.Errors.ScrapingError;

namespace Graber.Infrastructure.Scrapers;

public class XScraper : IScraper
{
    public bool CanExecute(string input)
    {
        return input.Contains("x.com");
    }

    public async Task<Result<string>> ExecuteAsync(string input)
    {
        var playlistUrl = await GetPlaylistUrlAsync(input);
        
        if (playlistUrl.IsFailure)
            return Result<string>.Failure(playlistUrl.Error);
        
        return Result<string>.Success(playlistUrl.Value);
    }

    public async Task<Result<string>> GetPlaylistUrlAsync(string input)
    {
        try
        {
            await new BrowserFetcher().DownloadAsync();

            await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
            {
                Headless = false
            });

            await using var page = await browser.NewPageAsync();

            var playlistTask = page.WaitForResponseAsync(response =>
                response.Url.Contains(".m3u8", StringComparison.OrdinalIgnoreCase));

            await page.GoToAsync(input, new NavigationOptions()
            {
                WaitUntil = [WaitUntilNavigation.DOMContentLoaded]
            });

            var response = await playlistTask.WaitAsync(TimeSpan.FromSeconds(15));
            var playlistUrl = response.Url;

            if (string.IsNullOrEmpty(playlistUrl))
            {
                return Result<string>.Failure(new ScrapingError(ScrapingErrorCode.MediaNotFound));
            }

            return Result<string>.Success(playlistUrl);
        }
        catch (TimeoutException)
        {
            return Result<string>.Failure(new ScrapingError(ScrapingErrorCode.MediaNotFound));
        }
        catch (Exception)
        {
            return Result<string>.Failure(new ScrapingError(ScrapingErrorCode.MediaDiscoveryFailed));
        }
    }
}