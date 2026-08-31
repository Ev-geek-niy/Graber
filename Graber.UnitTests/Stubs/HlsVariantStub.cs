using Graber.Infrastructure.Hls;

namespace Graber.UnitTests.Stubs;

public class HlsVariantStub : HlsVariant
{
    public HlsVariantStub(int bandwidth, int? averageBandwidth = null)
    {
        VideoUrl = new Uri("/test/uri", UriKind.RelativeOrAbsolute);
        Bandwidth = bandwidth;
        AverageBandwidth = averageBandwidth;
    }
}