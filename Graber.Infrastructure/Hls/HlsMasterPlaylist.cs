namespace Graber.Infrastructure.Hls;

public class HlsMasterPlaylist
{
    public Uri PlaylistUrl { get; init; }
    public IEnumerable<HlsVariant>  Variants { get; init; }
}