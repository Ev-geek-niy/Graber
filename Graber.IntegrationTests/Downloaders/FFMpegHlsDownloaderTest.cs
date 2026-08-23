using Graber.Application.Errors;
using Graber.Infrastructure.Downloaders;

namespace Graber.IntegrationTests.Downloaders;

public class FFMpegHlsDownloaderTest
{
    [Fact]
    public async Task ExecuteAsync_WhenDownloadSucceeds_ReturnsNonEmptyStream()
    {
        using var stream = new MemoryStream();
        var mediaBufferFactory = new StubMediaBufferFactory(stream);
        var downloader = new FFMpegHlsDownloader(mediaBufferFactory);
        var result = await downloader.ExecuteAsync(
            "https://video.twimg.com/amplify_video/2080134653969674240/pl/slL7C-ESevTgXXL0.m3u8?tag=29&v=cfc",
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(stream, result.Value);
        Assert.True(result.Value.Length > 0);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDownloadFails_ReturnsFailureAndDisposesBuffer()
    {
        using var stream = new MemoryStream();
        var mediaBufferFactory = new StubMediaBufferFactory(stream);
        var downloader = new FFMpegHlsDownloader(mediaBufferFactory);
        var result = await downloader.ExecuteAsync("file://localhost",
            CancellationToken.None);
        
        Assert.True(result.IsFailure);
        Assert.Equal(new DownloadError(DownloadErrorCode.DownloadFailed), result.Error);
        Assert.False(stream.CanRead);
    }
    
    [Fact]
    public async Task ExecuteAsync_WhenUriIsInvalid_ReturnsFailureAndDisposesBuffer()
    {
        using var stream = new MemoryStream();
        var mediaBufferFactory = new StubMediaBufferFactory(stream);
        var downloader = new FFMpegHlsDownloader(mediaBufferFactory);

        await Assert.ThrowsAnyAsync<UriFormatException>(() => downloader.ExecuteAsync("not-an-absolute-url",
            CancellationToken.None));
        Assert.False(stream.CanRead);
    }
    
    [Fact]
    public async Task ExecuteAsync_WhenCancelled_ThrowsOperationCanceledExceptionAndDisposesBuffer()
    {
        using var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        using var stream = new MemoryStream();
        var mediaBufferFactory = new StubMediaBufferFactory(stream);
        var downloader = new FFMpegHlsDownloader(mediaBufferFactory);


        await Assert.ThrowsAnyAsync<OperationCanceledException>(() =>
            downloader.ExecuteAsync("file://localhost",cts.Token));
        Assert.False(stream.CanRead);
    }
}