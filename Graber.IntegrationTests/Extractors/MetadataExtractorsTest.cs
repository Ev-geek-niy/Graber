using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Extractors;
using Graber.Infrastructure.Scrapers;
using Xunit.Abstractions;

namespace Graber.IntegrationTests.Extractors;

public class MetadataExtractorsTest(ITestOutputHelper output)
{
    [Theory]
    [InlineData("https://x.com/philosophymeme0/status/2080134676878967139?s=20")]
    [InlineData("https://x.com/rootpilot/status/2083280043531452776?s=20")]
    [InlineData("https://x.com/MemoryOffline/status/2082300088064618924?s=20")]
     public async Task MetadataExtractor_Extract_IsSuccessful(string url)
    {
        var downloader = new FFMpegHlsDownloader();
        var extractor = new MetadataExtractor();
        var scraper = new XScraper();
        
        var playlistUrlResult = await scraper.GetPlaylistUrlAsync(url);
        Assert.True(playlistUrlResult.IsSuccess);
        
        var streamResult = await downloader.ExecuteAsync(playlistUrlResult.Value);
        Assert.True(streamResult.IsSuccess);

        await using var value = streamResult.Value;
        
        var metadataResult = await extractor.ExtractAsync(value);
        Assert.True(metadataResult.IsSuccess);
        Assert.NotNull(metadataResult.Value);
        
        output.WriteLine($"Resolution: {metadataResult.Value.Width}x{metadataResult.Value.Height}");
    }
}