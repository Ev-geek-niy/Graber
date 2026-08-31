using Graber.Infrastructure.Hls.Models;

namespace Graber.Infrastructure.Hls;

public class HlsSelector
{
    public IReadOnlyList<HlsVariant> SelectCandidates(IReadOnlyList<HlsVariant> variants)
    {
        if (variants.Count == 0)
            return [];

        var orderedVariants = variants
            .OrderByDescending(variant => variant.AverageBandwidth ?? variant.Bandwidth)
            .ToList();
        var medianIndex = orderedVariants.Count / 2;
        return [..orderedVariants.Skip(medianIndex)];
    }
}
