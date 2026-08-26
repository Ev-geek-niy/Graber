using Graber.Infrastructure.Hls;

namespace Graber.UnitTests;

public class HlsPlaylistParserTest
{
    private Uri PlaylistUri => new Uri("https://video.twimg.com/amplify_video/2080134653969674240/pl/slL7C-ESevTgXXL0.m3u8?tag=29&v=cfc",  UriKind.Absolute);
    private const string VideoTag = "#EXT-X-STREAM-INF";
    private const string AudioTag = "#EXT-X-MEDIA";
    
    [Fact]
    public async Task Parse_WhenPlaylistHave3Variants_Return3Variants()
    {
        var playlistContent = await File.ReadAllTextAsync(Path.Combine("..", "..", "..", "Fixtures", "Hls", "x-playlist.m3u8"));
        var parser = new HlsPlaylistParser();

        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);

        Assert.Equal(3, masterPlaylist.Variants.Count());
    }

    [Fact]
    public async Task Parse_WhenPlaylistHave3Audio_Returns3Audio()
    {
        var playlistContent = await File.ReadAllTextAsync(Path.Combine("..", "..", "..", "Fixtures", "Hls", "x-playlist.m3u8"));
        var parser = new HlsPlaylistParser();

        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);

        Assert.Equal(3, masterPlaylist.AudioRenditions.Count);
    }

    [Fact]
    public async Task Parse_WhenSecondVariantExists_ReturnsItsAttributes()
    {
        var playlistContent = await File.ReadAllTextAsync(Path.Combine("..", "..", "..", "Fixtures", "Hls", "x-playlist.m3u8"));
        var parser = new HlsPlaylistParser();
        var secondVariant = new HlsVariant()
        {
            VideoUrl = new Uri("/amplify_video/2080134653969674240/pl/avc1/368x360/7jgUf4T2IP03fqQ6.m3u8",  UriKind.RelativeOrAbsolute),
            AudioGroupId = "audio-64000",
            AverageBandwidth = 208756,
            Bandwidth = 227990,
            Codecs = ["mp4a.40.2", "avc1.4D4015"],
            Resolution = new Resolution { Width = 368,  Height = 360 }
        };

        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);
        var targetVariant = masterPlaylist.Variants[1];
        
        Assert.Equal(secondVariant.VideoUrl, targetVariant.VideoUrl);
        Assert.Equal(secondVariant.AudioGroupId, targetVariant.AudioGroupId);
        Assert.Equal(secondVariant.AverageBandwidth, targetVariant.AverageBandwidth);
        Assert.Equal(secondVariant.Bandwidth, targetVariant.Bandwidth);
        Assert.Equal(secondVariant.Codecs, targetVariant.Codecs);
        Assert.Equal(secondVariant.Resolution, targetVariant.Resolution);
    }

    [Fact]
    public async Task Parse_WhenPlaylistContainsAudioRendition_ReturnsItsAttributes()
    {
        var playlistContent = await File.ReadAllTextAsync(Path.Combine("..", "..", "..", "Fixtures", "Hls", "x-playlist.m3u8"));
        var parser = new HlsPlaylistParser();
        var secondAudioRendition = new AudioRendition()
        {
            GroupId = "audio-64000",
            Uri = new Uri("/amplify_video/2080134653969674240/pl/mp4a/64000/ju_NNmKAwDZkQzcs.m3u8", UriKind.Relative),
            AutoSelect = true,
            Name = "Audio"
        };
        
        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);
        
        Assert.NotNull(masterPlaylist.AudioRenditions[1]);
        var targetAudioRendition = masterPlaylist.AudioRenditions[1];
        Assert.Equal(secondAudioRendition.Name, targetAudioRendition.Name);
        Assert.Equal(secondAudioRendition.GroupId, targetAudioRendition.GroupId);
        Assert.Equal(secondAudioRendition.Uri, targetAudioRendition.Uri);
        Assert.Equal(secondAudioRendition.AutoSelect, targetAudioRendition.AutoSelect);
    }

    [Fact]
    public void Parse_WhenPlaylistHasNoAverageBandwidth_ReturnsNull()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:BANDWIDTH=88937
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();
        
        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);
        var hlsVariant = masterPlaylist.Variants[0];
        
        Assert.Null(hlsVariant.AverageBandwidth);
    }

    [Fact]
    public void Parse_WhenResolutionIsNull_ReturnsNull()
    {
            var playlistContent = """
                                  #EXTM3U
                                  #EXT-X-STREAM-INF:BANDWIDTH=88937
                                  /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                                  """;
            var parser = new HlsPlaylistParser();
        
            var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);
            var hlsVariant = masterPlaylist.Variants[0];
        
            Assert.Null(hlsVariant.Resolution);
    }

    [Fact]
    public void Parse_WhenAudioIsNull_ReturnsNull()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:BANDWIDTH=88937
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();
        
        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);
        var hlsVariant = masterPlaylist.Variants[0];
        
        Assert.Null(hlsVariant.AudioGroupId);
    }

    [Fact]
    public void Parse_WhenAverageBandwidthIsZero_ThrowsFormatException()
    {
        var playlistContent = """
                         #EXTM3U
                         #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=0,BANDWIDTH=10,RESOLUTION=100x100
                         /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                         """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
        
        Assert.Equal("#EXT-X-STREAM-INF attribute AVERAGE-BANDWIDTH must be greater than 0.", exception.Message);
    }

    [Fact]
    public void Parse_WhenAverageBandwidthIsNegative_ThrowsFormatException()
    {
        var playlistContent = """
                         #EXTM3U
                         #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=-10,BANDWIDTH=10,RESOLUTION=100x100
                         /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                         """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
        
        Assert.Equal("#EXT-X-STREAM-INF attribute AVERAGE-BANDWIDTH must be greater than 0.", exception.Message);
    }

    [Fact]
    public void Parse_WhenBandwidthIsZero_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=0,RESOLUTION=100x100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute BANDWIDTH must be greater than 0.", exception.Message);
    }
    
    [Fact]
    public void Parse_WhenBandwidthIsNegative_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=-10,RESOLUTION=100x100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute BANDWIDTH must be greater than 0.", exception.Message);
    }

    [Fact]
    public void Parse_WhenResolutionHasOneValue_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=asd100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute RESOLUTION must contain 2 values.", exception.Message);
    }
    
    [Fact]
    public void Parse_WhenResolutionHasFirstInvalidValue_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=asdx100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute RESOLUTION values 'asdx100' must be integers.", exception.Message);
    }
    
    [Fact]
    public void Parse_WhenResolutionHasSecondInvalidValue_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=100xqqq
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute RESOLUTION values '100xqqq' must be integers.", exception.Message);
    }
    
    [Fact]
    public void Parse_WhenResolutionHasFirstNegativeValue_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=-100x100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute RESOLUTION values '-100x100' must be greater than zero.", exception.Message);
    }
    
    [Fact]
    public void Parse_WhenResolutionHaSecondNegativeValue_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=100x-100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("#EXT-X-STREAM-INF attribute RESOLUTION values '100x-100' must be greater than zero.", exception.Message);
    }

    [Fact]
    public void Parse_WhenNoStartingTag_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=100x-100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("HLS playlist content must start with '#EXTM3U'.", exception.Message);
    }
}