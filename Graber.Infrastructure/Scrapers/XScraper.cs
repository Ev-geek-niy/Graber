using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using PuppeteerSharp;

namespace Graber.Infrastructure.Scrapers;

public class XScraper(IMediaDownloader downloader, IMetadataExtractor extractor) : IScraper
{
    public bool CanExecute(string input)
    {
        return input.Contains("x.com");
    }

    public async Task<Result<Video>> ExecuteAsync(string input)
    {
        var playlistUrl = await GetPlaylistUrlAsync(input);
        
        if (playlistUrl.IsFailure)
            return Result.Failure(playlistUrl.Error);
        
        var outputStream = await downloader.ExecuteAsync(playlistUrl.Value);
        if (outputStream.IsFailure)
            return Result.Failure(outputStream.Error);
        
        var metadata = await extractor.ExtractAsync(outputStream.Value);
        if  (metadata.IsFailure)
            return Result.Failure(metadata.Error);
        
        return Result.Success(new Video(outputStream.Value,  metadata.Value));
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
                return Result.Failure(ScrapingErrorType.NotFoundVideo);
            }

            return Result.Success(playlistUrl);
        }
        catch (TimeoutException)
        {
            return Result.Failure(ScrapingErrorType.NotFoundVideo);
        }
        catch (Exception ex)
        {
            return Result.Failure(ScrapingErrorType.NetworkError, ex.Message);
        }
    }
}