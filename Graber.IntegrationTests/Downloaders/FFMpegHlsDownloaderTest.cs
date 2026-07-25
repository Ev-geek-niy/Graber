using Graber.Infrastructure.Downloaders;

namespace Graber.IntegrationTests.Downloaders;

public class FFMpegHlsDownloaderTest
{
    [Fact]
    public async Task FFMpegHlsDownloader_GetOutputStream_ShouldReturnStream()
    {
        var downloader = new FFMpegHlsDownloader();
        var result = await downloader.ExecuteAsync("https://video.twimg.com/amplify_video/2080134653969674240/pl/mp4a/64000/ju_NNmKAwDZkQzcs.m3u8");
        
        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Length > 0);
    }
}