using System.Text.RegularExpressions;

namespace Graber.Infrastructure.Hls;

public class HlsPlaylistParser
{
    private readonly Regex attributeRegex = new Regex("""(?<name>[A-Z0-9-]+)=(?:"(?<quotedValue>[^"]*)"|(?<value>[^,]*))""");
    public HlsMasterPlaylist Parse(string playlistContent, Uri playlistUrl)
    {
        var audioGroups = new List<Dictionary<string, string>>();
        var videoGroups = new List<Dictionary<string, string>>();
        var videoGroupPreviousLine = false;
        
        var lines = playlistContent.Split("\n");
        foreach (var line in lines)
        {
            if (DefineType(line) == HlsTypeEnum.Audio)
                audioGroups.Add(ParseAttributes(line));
            else if (DefineType(line) == HlsTypeEnum.Video)
            {
                videoGroups.Add(ParseAttributes(line));
                videoGroupPreviousLine = true;
            }
            else if (videoGroupPreviousLine)
            {
                var videoGroup = videoGroups.LastOrDefault();
                videoGroup?.TryAdd("URI", line);
                videoGroupPreviousLine = false;
            }
        }

        var variants = videoGroups
            .Select(videoGroup =>
            {
                var audio = audioGroups.Find(audioGroup => videoGroup["AUDIO"] == audioGroup["GROUP-ID"]);
                return new HlsVariant()
                {
                    AbsoluteAudioUrl = new Uri(audio.GetValueOrDefault("URI")),
                    AbsoluteVideoUrl = new Uri(videoGroup.GetValueOrDefault("URI")),
                    AverageBandwidth = int.Parse(videoGroup.GetValueOrDefault("AVERAGE-BANDWIDTH")),
                    Bandwidth = int.Parse(videoGroup.GetValueOrDefault("BANDWIDTH")),
                    Codecs = videoGroup.GetValueOrDefault("CODECS").Split(","),
                    Height = int.Parse(videoGroup.GetValueOrDefault("HEIGHT")),
                    Width = int.Parse(videoGroup.GetValueOrDefault("WIDTH")),
                };
            });

        return new HlsMasterPlaylist()
        {
            Variants = variants,
            PlaylistUrl = playlistUrl,
        };
    }

    private Dictionary<string, string> ParseAttributes(string playlistContentLine)
    {
        var attributes = new Dictionary<string, string>();
        var matches = attributeRegex.Matches(playlistContentLine);
        foreach (Match match in matches)
        {
            if (match.Groups["quotedValue"].Success)
                attributes.TryAdd(match.Groups["name"].Value, match.Groups["quotedValue"].Value);
            if (match.Groups["value"].Success)
                attributes.TryAdd(match.Groups["name"].Value, match.Groups["value"].Value);
        }
        
        return attributes;
    }

    private HlsTypeEnum DefineType(string playlist)
    {
        if (playlist.StartsWith("#EXT-X-MEDIA")) return HlsTypeEnum.Audio;
        if (playlist.StartsWith("#EXT-X-STREAM-INF")) return HlsTypeEnum.Video;

        return HlsTypeEnum.None;
    }
    
    private enum HlsTypeEnum
    {
        None,
        Audio,
        Video
    } 
}

