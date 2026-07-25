using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Extractors;
using Graber.Infrastructure.Scrapers;

namespace Graber.IntegrationTests.Extractors;

public class MetadataExtractorsTest
{
    [Fact]
    public async Task MetadataExtractor_Extract_IsSuccessful()
    {
        var downloader = new FFMpegHlsDownloader();
        var scraper = new XScraper(downloader);
        var extractor = new MetadataExtractor();

        var playlistUrlResult = await scraper.GetPlaylistUrlAsync("https://x.com/philosophymeme0/status/2080134676878967139?s=20");
        var streamResult = await downloader.ExecuteAsync(playlistUrlResult.Value);
        var metadataResult = await extractor.ExtractAsync(streamResult.Value);
        
        Assert.True(metadataResult.IsSuccess);
        Assert.NotNull(metadataResult.Value);
    }
}