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
                Headless = true
            });

            await using var page = await browser.NewPageAsync();

            string? playlistUrl = null;
            page.Response += (_, e) =>
            {
                if (e.Response.Url.Contains(".m3u8", StringComparison.OrdinalIgnoreCase))
                {
                    playlistUrl ??= e.Response.Url;
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
        catch (Exception ex)
        {
            return Result.Failure(ScrapingErrorType.NetworkError, ex.Message);
        }
    }
}