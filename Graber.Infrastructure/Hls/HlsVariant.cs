namespace Graber.Infrastructure.Hls;

public class HlsVariant
{
    public required Uri VideoUrl
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
