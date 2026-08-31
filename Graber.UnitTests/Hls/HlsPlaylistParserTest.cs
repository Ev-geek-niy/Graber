using Graber.Infrastructure.Hls;
using Graber.Infrastructure.Hls.Models;

namespace Graber.UnitTests.Hls;

public class HlsPlaylistParserTest
{
    private Uri PlaylistUri => new Uri("https://video.twimg.com/amplify_video/2080134653969674240/pl/slL7C-ESevTgXXL0.m3u8?tag=29&v=cfc",  UriKind.Absolute);
    
    [Fact]
    public async Task Parse_WhenPlaylistHas3Variants_Return3Variants()
    {
        var playlistContent = await File.ReadAllTextAsync(Path.Combine("..", "..", "..", "Fixtures", "Hls", "x-playlist.m3u8"));
        var parser = new HlsPlaylistParser();

        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);

        Assert.Equal(3, masterPlaylist.Variants.Count());
    }

    [Fact]
    public async Task Parse_WhenPlaylistHas3Audio_Returns3Audio()
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
    public void Parse_WhenPlaylistDoesNotHaveVideoTag_ThrowsFormatException()
    {
        var playlistContent = $"""
                               #EXTM3U
                               #EXT-X-VERSION:6
                               """;
        var parser = new HlsPlaylistParser();
        
        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-100")]
    [InlineData("abc")]
    
    public void Parse_WhenAverageBandwidthIsInvalid_ThrowsFormatException(string averageBandwidth)
    {
        var playlistContent = $"""
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH={averageBandwidth},BANDWIDTH=10,RESOLUTION=100x100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-100")]
    [InlineData("abc")]
    [InlineData(null)]
    public void Parse_WhenBandwidthInvalid_ThrowsFormatException(string? bandwidth)
    {
        var playlistContent = $"""
                               #EXTM3U
                               #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=1000,BANDWIDTH={bandwidth},RESOLUTION=100x100
                               /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                               """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("abcx123")]
    [InlineData("123xabc")]
    [InlineData("-123x123")]
    [InlineData("123x-123")]
    
    public void Parse_WhenResolutionInvalid_ThrowsFormatException(string resolution)
    {
        var playlistContent = $"""
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION={resolution}
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();
        
        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }
    
    [Theory]
    [InlineData("EXTM3U")]
    [InlineData("#EXTM3U-invalid")]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("    ")]
    public void Parse_WhenPlaylistHeader_ThrowsFormatException(string? playlistHeader)
    {
        var playlistContent = $"""
                              {playlistHeader}
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=10,BANDWIDTH=10,RESOLUTION=100x100
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }
    
    [Theory]
    [InlineData("GROUP-ID=")]
    [InlineData("GROUP-ID=   ")]
    [InlineData("GROUP-ID=\"\"")]
    [InlineData("GROUP-ID=\"     \"")]
    public void Parse_WhenAudioRenditionGroupIdIsMissingOrEmpty_ThrowsFormatException(string? requiredString)
    {
        var playlistContent = $"""
                              #EXTM3U
                              #EXT-X-MEDIA:NAME="Audio",TYPE=AUDIO,{requiredString},AUTOSELECT=YES,URI="/amplify_video/2080134653969674240/pl/mp4a/32000/is8JNmTlua5W6KJw.m3u8"
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }

    [Fact]
    public void Parse_WhenFileDoesNotHaveTag_ThrowsFormatException()
    {
        var playlistContent = """
                              #EXTM3U
                              EXT-X-MEDIA:NAME="Audio",TYPE=AUDIO,GROUP-ID="audio-128000",AUTOSELECT=YES,URI="/amplify_video/2080134653969674240/pl/mp4a/128000/fpzyuyxNwT8BA3E3.m3u8"
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        var exception = Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));

        Assert.Equal("Line does not contain a valid HLS tag.", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("\n")]
    [InlineData("#amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8")]
    public void Parse_WhenHlsVariantUri_ThrowsFormatException(string? hlsVariantUri)
    {
        var playlistContent = $"""
                              #EXTM3U
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                              {hlsVariantUri}
                              """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }
    
    [Theory]
    [InlineData("URI=")]
    [InlineData("URI=\"\"")]
    [InlineData("URI=    ")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    public void Parse_WhenAudioRenditionUri_ThrowsFormatException(string? audioRenditionUri)
    {
        var playlistContent = $"""
                              #EXTM3U
                              #EXT-X-MEDIA:NAME="Audio",TYPE=AUDIO,GROUP-ID="audio-128000",AUTOSELECT=YES,{audioRenditionUri}
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }
    
    [Fact]
    public void Parse_WhenMediaTypeIsNotAudio_DoesNotAddAudioRendition()
    {
        var playlistContent = """
                              #EXTM3U
                              #EXT-X-MEDIA:NAME="Subtitles",TYPE=SUBTITLES,GROUP-ID="audio-32000",AUTOSELECT=YES,URI="/amplify_video/2080134653969674240/pl/mp4a/32000/is8JNmTlua5W6KJw.m3u8"
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();
        
        var result = parser.Parse(playlistContent, PlaylistUri);

        Assert.NotEmpty(result.Variants);
        Assert.Empty(result.AudioRenditions);
    }

    [Theory]
    [InlineData("YES", true)]
    [InlineData("NO", false)]
    public void Parse_WhenAutoSelectIsYesOrNo_ReturnsExpectedValue(string value, bool expectedResult)
    {
        var playlistContent = $"""
                              #EXTM3U
                              #EXT-X-MEDIA:NAME="Subtitles",TYPE=AUDIO,GROUP-ID="audio-32000",AUTOSELECT={value},URI="/amplify_video/2080134653969674240/pl/mp4a/32000/is8JNmTlua5W6KJw.m3u8"
                              #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                              /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                              """;
        var parser = new HlsPlaylistParser();
        
        var masterPlaylist = parser.Parse(playlistContent, PlaylistUri);
        var audio = masterPlaylist.AudioRenditions[0];
        
        Assert.Equal(expectedResult, audio.AutoSelect);
    }

    [Theory]
    [InlineData("нет")]
    [InlineData("Да")]
    [InlineData("MAYBE")]
    [InlineData("Yes")]
    [InlineData("No")]
    public void Parse_WhenOptionalYesNo_ThrowsFormatException(string value)
    {
        var playlistContent = $"""
                               #EXTM3U
                               #EXT-X-MEDIA:NAME="Audio",TYPE=AUDIO,GROUP-ID="audio-32000",AUTOSELECT={value},URI="/amplify_video/2080134653969674240/pl/mp4a/32000/is8JNmTlua5W6KJw.m3u8"
                               #EXT-X-STREAM-INF:AVERAGE-BANDWIDTH=81403,BANDWIDTH=88937,RESOLUTION=276x270,CODECS="mp4a.40.2,avc1.4D400D",AUDIO="audio-32000"
                               /amplify_video/2080134653969674240/pl/avc1/276x270/uysqztpZbewRDxv8.m3u8
                               """;
        var parser = new HlsPlaylistParser();

        Assert.Throws<FormatException>(() => parser.Parse(playlistContent, PlaylistUri));
    }
}
