using Graber.Infrastructure.Hls;

namespace Graber.UnitTests;

public class HlsPlaylistParserTest
{
    [Fact]
    public async Task Parse_WhenPlaylistHave3Variants_Return3Variants()
    {
        var playlistContent = await File.ReadAllTextAsync(Path.Combine("..", "..", "..", "Fixtures", "Hls", "x-playlist.m3u8"));
        var parser = new HlsPlaylistParser();

        var masterPlaylist = parser.Parse(playlistContent, new Uri("https://video.twimg.com/amplify_video/2080134653969674240/pl/slL7C-ESevTgXXL0.m3u8?tag=29&v=cfc"));

        Assert.Equal(3, masterPlaylist.Variants.Count());
    }
}