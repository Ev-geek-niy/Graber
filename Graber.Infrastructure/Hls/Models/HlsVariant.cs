namespace Graber.Infrastructure.Hls.Models;

public class HlsVariant
{
    public Uri VideoUrl
    {
        get;
        init
        {
            ArgumentNullException.ThrowIfNull(value);
            field = value;
        }
    }

    public string? AudioGroupId { get; init; }
    public int? AverageBandwidth { get; init; }

    public int Bandwidth
    {
        get;
        init
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            field = value;
        }
    }

    public Resolution? Resolution { get; init; }
    public IReadOnlyList<string>? Codecs { get; init; }
}
