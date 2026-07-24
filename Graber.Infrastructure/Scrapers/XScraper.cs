using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using PuppeteerSharp;

namespace Graber.Infrastructure.Scrapers;

public class XScraper : IScraper
{
    public bool CanExecute(string input)
    {
        return input.Contains("x.com");
    }

    public async Task<Result<Video>> ExecuteAsync(string input)
    {
        var playlistUrl = await GetPlaylistUrlAsync(input);
        // код дальше
        return Result.Failure(ScrapingErrorType.DeleteVideo);
    }

    public async Task<Result<string>> GetPlaylistUrlAsync(string input)
    {
        await new BrowserFetcher().DownloadAsync();

        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions()
        {
            Headless = true
        });

        await using var page = await browser.NewPageAsync();
        
        string? playlistUrl = null;
        page.Response += (_, e) =>
        {
            if (e.Response.Url.Contains(".m3u8"))
            {
                playlistUrl = e.Response.Url;
            }
        };

        await page.GoToAsync(input, new NavigationOptions()
        {
            WaitUntil = [WaitUntilNavigation.Networkidle2]
        });

        if (string.IsNullOrEmpty(playlistUrl))
        {
            return Result.Failure(ScrapingErrorType.NotFoundVideo);
        }

        return Result.Success(playlistUrl);
    }
}