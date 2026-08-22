using Graber.Application.Errors;
using Graber.Infrastructure.Scrapers;

namespace Graber.IntegrationTests.Scrapers;

public class XScraperTest
{
    [Fact]
    public async Task XScraper_GetPlaylistUrl_SuccessResult()
    {
        var xScraper = new XScraper();
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/2080134676878967139?s=20", CancellationToken.None);
        
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task XScraper_GetPlaylistUrl_NotFoundResult()
    {
        var xScraper = new XScraper();
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/392912", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new ScrapingError(ScrapingErrorCode.MediaNotFound),  result.Error);
    }
    
    [Fact]
    public async Task XScraper_GetPlaylistUrl_MediaDiscoveryResult()
    {
        var xScraper = new XScraper();
        var result = await xScraper.GetPlaylistUrlAsync("https://127.0.0.1:1", CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(new ScrapingError(ScrapingErrorCode.MediaDiscoveryFailed),  result.Error);
    }

    [Fact]
    public async Task XScraper_InspectPlaylist()
    {
        var xScraper = new XScraper();
        var result = await xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/2080134676878967139?s=20", CancellationToken.None);
        Assert.True(result.IsSuccess);
        
        using var httpClient = new HttpClient();
        var playlistContent = await httpClient.GetStringAsync(result.Value, CancellationToken.None);
        
        var solutionRoot = FindSolutionRoot();
        var researchDirectory = Path.Combine(solutionRoot, ".research");

        Directory.CreateDirectory(researchDirectory);
        var playlistPath = Path.Combine(researchDirectory, "x-playlist.m3u8");
        await File.WriteAllTextAsync(playlistPath, playlistContent, CancellationToken.None);
    }

    [Fact]
    public async Task XScraper_CancelOperation_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        var xScraper = new XScraper();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            xScraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/392912", cts.Token));
    } 

    private static string FindSolutionRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null)
        {
            if (File.Exists(Path.Combine(directory.FullName, "Graber.sln")))
                return directory.FullName;
            directory = directory.Parent;
        }

        throw new DirectoryNotFoundException(
            "Не удалось найти корень проекта");
    }
}