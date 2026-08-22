using Graber.Application.Errors;
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
        var stream = new MemoryStream();
        var mediaBufferFactory = new StubMediaBufferFactory(stream);
        var downloader = new FFMpegHlsDownloader(mediaBufferFactory);
        var extractor = new MetadataExtractor();
        var scraper = new XScraper();
        
        var playlistUrlResult = await scraper.GetPlaylistUrlAsync(url, CancellationToken.None);
        Assert.True(playlistUrlResult.IsSuccess);
        
        var streamResult = await downloader.ExecuteAsync(playlistUrlResult.Value, CancellationToken.None);
        Assert.True(streamResult.IsSuccess);

        await using var value = streamResult.Value;
        
        var metadataResult = await extractor.ExtractAsync(value, CancellationToken.None);
        Assert.True(metadataResult.IsSuccess);
        Assert.NotNull(metadataResult.Value);
        
        output.WriteLine($"Resolution: {metadataResult.Value.Width}x{metadataResult.Value.Height}");
    }
     
     [Fact]
     public async Task ExtractAsync_WhenCancelled_RethrowsAndRestoresStreamPosition()
     {
         const int expectedStreamPosition = 1;
         using var stream = new MemoryStream([1, 2, 3]);
         stream.Position = expectedStreamPosition;
         
         using var cts = new CancellationTokenSource();
         cts.Cancel();
         
         var extractor = new MetadataExtractor();
         
         await Assert.ThrowsAnyAsync<OperationCanceledException>(() => 
             extractor.ExtractAsync(stream, cts.Token));
         
         Assert.Equal(expectedStreamPosition,  stream.Position);
     }

     [Fact]
     public async Task ExtractAsync_WhenMediaIsInvalid_ReturnsExtractionFailure()
     {
         await using var stream = new MemoryStream(
             [.. "This is not a media file"u8]);
         var extractor = new MetadataExtractor();
         
         var result = await extractor.ExtractAsync(stream, CancellationToken.None);
         
         Assert.True(result.IsFailure);
         Assert.Equal(
             new MetadataError(MetadataErrorCode.ExtractionFailed),
             result.Error);
     }

     [Fact]
     public async Task ExtractAsync_WhenMediaIsEmpty_ReturnsExtractionFailure()
     {
         await using var stream = new MemoryStream([]);
         var extractor = new MetadataExtractor();
         
         var result = await extractor.ExtractAsync(stream, CancellationToken.None);
         
         Assert.True(result.IsFailure);
         Assert.Equal(
             new MetadataError(MetadataErrorCode.ExtractionFailed),
             result.Error);
     }
}