namespace Graber.Infrastructure.Hls;

public class HlsVariant
{
    public Uri AbsoluteVideoUrl { get; init; }
    public Uri AbsoluteAudioUrl { get; init; }
    public int AverageBandwidth { get; init; }
    public int Bandwidth { get; init; }
    public int Width { get; init; }
    public int Height { get; init; }
    public IEnumerable<string> Codecs { get; init; }
}
