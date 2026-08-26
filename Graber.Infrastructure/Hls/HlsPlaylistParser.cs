namespace Graber.Infrastructure.Hls;

public class HlsPlaylistParser
{
    public HlsMasterPlaylist Parse(string playlistContent, Uri playlistUrl)
    {
        if (!playlistContent.StartsWith("#EXTM3U"))
            throw new FormatException("HLS playlist content must start with '#EXTM3U'.");
        
        var hlsVariants = new List<HlsVariant>();
        var audioRenditions = new List<AudioRendition>();

        var lines = playlistContent.Split("\n");

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                continue;
            
            var attributes = HlsAttributes.Parse(lines[i]);
            switch (attributes.Tag)
            {
                case "#EXT-X-MEDIA":
                    if (attributes.RequiredString("TYPE") != "AUDIO")
                        continue;
                    audioRenditions.Add(new AudioRendition
                    {
                        GroupId = attributes.RequiredString("GROUP-ID"),
                        Uri = attributes.RequiredUri("URI"),
                        Name = attributes.OptionalString("NAME"),
                        AutoSelect = attributes.OptionalYesNo("AUTOSELECT"),
                        IsDefault = attributes.OptionalYesNo("DEFAULT"),
                        Language = attributes.OptionalString("LANGUAGE"),
                    });
                    break;

                case "#EXT-X-STREAM-INF":
                    if (i + 1 >= lines.Length)
                        throw new FormatException("Unexpected end of file.");

                    if (string.IsNullOrWhiteSpace(lines[i + 1]))
                        throw new FormatException("Line must not be empty.");
                    
                    if (lines[i + 1].Trim().StartsWith('#'))
                        throw new FormatException("Line must not start with '#'.");
                    
                    if (!Uri.TryCreate(lines[i + 1].Trim(), UriKind.RelativeOrAbsolute, out var variantUri))
                        throw new FormatException($"Next line of file must be an URI. Next line: {lines[i + 1]}");

                    hlsVariants.Add(new()
                    {
                        VideoUrl = variantUri,
                        AudioGroupId = attributes.OptionalString("AUDIO"),
                        AverageBandwidth = attributes.OptionalPositiveInt("AVERAGE-BANDWIDTH"),
                        Bandwidth = attributes.RequiredPositiveInt("BANDWIDTH"),
                        Codecs = attributes.OptionalString("CODECS")?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries),
                        Resolution = attributes.OptionalResolution("RESOLUTION")
                    });

                    // Skip uri line
                    i += 1;
                    break;
                default:
                    continue;
            }
        }
        
        return new HlsMasterPlaylist()
        {
            Variants = hlsVariants,
            AudioRenditions = audioRenditions,
            PlaylistUrl = playlistUrl,
        };
    }
}



