using Graber.Application.Enums;
using Graber.Application.Models;
using Graber.Application.Providers;
using Graber.Application.UseCases;
using Graber.Domain.Models;

namespace Graber.UnitTests;

public class ProcessUrlUseCaseTest
{
    private string VideoUrl => "https://x.com/philosophymeme0/status/2080134676878967139?s=20";
    private string TestPlaylistUrl =>
        "https://video.twimg.com/amplify_video/2080134653969674240/pl/slL7C-ESevTgXXL0.m3u8?tag=29&v=cfc";
    
    [Fact]
    public async Task ExecuteAsync_WhenAllStepsSucceed_ReturnsVideo()
    {
        using var stream = new MemoryStream();
        var metadata = new VideoMetadata("Testname", "mp4", "video/mp4", TimeSpan.FromSeconds(10), 200, 200);
        
        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(true, TestPlaylistUrl)
        ]);
        var downloader = new MediaDownloaderProvider([new StubHlsDownloader(true, stream)]);
        var extractor = new StubExtractor(metadata);
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var result = await useCase.ExecuteAsync(VideoUrl);
        Assert.True(result.IsSuccess);
        Assert.Same(stream, result.Value.VideoStream);
        Assert.Same(metadata, result.Value.Metadata);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoScraperSupportsUrl_ReturnsServiceNotSupportedFailure()
    {
        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(false, TestPlaylistUrl)
        ]);
        var downloader = new MediaDownloaderProvider([]);
        var extractor = new StubExtractor();
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var result = await useCase.ExecuteAsync(VideoUrl);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ScrapingErrorType.ServiceNotSupported, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenScraperFails_ReturnsScraperFailure()
    {
        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(true, "TestValue", new ScrapingError(ScrapingErrorType.NotFoundVideo, "Not found video error"))
        ]);
        var downloader = new MediaDownloaderProvider([]);
        var extractor = new StubExtractor();
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var result = await useCase.ExecuteAsync(VideoUrl);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ScrapingErrorType.NotFoundVideo, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenNoDownloaderSupportsUrl_ReturnsServiceNotSupportedFailure()
    {
        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(true, TestPlaylistUrl)
        ]);
        var downloader = new MediaDownloaderProvider([new StubHlsDownloader(false)]);
        var extractor = new StubExtractor();
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var result = await useCase.ExecuteAsync(VideoUrl);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ScrapingErrorType.ServiceNotSupported, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDownloaderFails_ReturnsDownloaderFailure()
    {
        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(true, TestPlaylistUrl)
        ]);
        var downloader = new MediaDownloaderProvider([new StubHlsDownloader(true, null, new ScrapingError(ScrapingErrorType.DeleteVideo, "Deleted video error"))]);
        var extractor = new StubExtractor();
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var result = await useCase.ExecuteAsync(VideoUrl);
        
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ScrapingErrorType.DeleteVideo, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExtractorFails_ReturnsExtractorFailureAndDisposesStream()
    {
        using var stream = new MemoryStream();
        var metadata = new VideoMetadata("Testname", "mp4", "video/mp4", TimeSpan.FromSeconds(10), 200, 200);

        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(true, TestPlaylistUrl)
        ]);
        var downloader = new MediaDownloaderProvider([new StubHlsDownloader(true, stream)]);
        var extractor = new StubExtractor(metadata, new ScrapingError(ScrapingErrorType.NotFoundVideo, "Not found video error"));
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var result = await useCase.ExecuteAsync(VideoUrl);
        
        Assert.False(stream.CanRead);
        Assert.True(result.IsFailure);
        Assert.NotNull(result.Error);
        Assert.Equal(ScrapingErrorType.NotFoundVideo, result.Error.Type);
    }

    [Fact]
    public async Task ExecuteAsync_WhenExtractorThrows_DisposesStreamAndRethrowsException()
    {
        var expectedException = new InvalidOperationException();
        using var stream = new MemoryStream();

        var scraperProvider = new ScraperProvider(
        [
            new StubScraper(true, TestPlaylistUrl)
        ]);
        var downloader = new MediaDownloaderProvider([new StubHlsDownloader(true, stream)]);
        var extractor = new ThrowingExtractor(expectedException);
        
        var useCase = new ProcessUrlUseCase(scraperProvider, extractor, downloader);
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => useCase.ExecuteAsync(VideoUrl));
        
        Assert.False(stream.CanRead);
        Assert.Same(expectedException, exception);
    }
}
