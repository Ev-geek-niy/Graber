using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Extractors;

namespace Graber.IntegrationTests.Downloaders;

public class FFMpegHlsDownloaderTest
{
    [Fact]
    public async Task FFMpegHlsDownloader_GetOutputStream_ShouldReturnStream()
    {
        var downloader = new FFMpegHlsDownloader();
        var result = await downloader.ExecuteAsync(
            "https://video.twimg.com/amplify_video/2080134653969674240/pl/slL7C-ESevTgXXL0.m3u8?tag=29&v=cfc");

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.Length > 0);
    }
}