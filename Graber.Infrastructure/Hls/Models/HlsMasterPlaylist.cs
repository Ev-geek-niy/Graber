namespace Graber.Infrastructure.Hls.Models;

public class HlsMasterPlaylist
{
    public required Uri PlaylistUrl { get; init; }
    public required IReadOnlyList<HlsVariant>  Variants { get; init; }
    public required IReadOnlyList<AudioRendition>  AudioRenditions { get; init; }
}