using Graber.Infrastructure.Hls;
using Graber.UnitTests.Stubs;

namespace Graber.UnitTests.Hls;

public class HlsSelectorTest
{
    [Theory]
    [InlineData(
        new[] {100, 200, 300, 400 },
        new[] {200, 100})]
    [InlineData(
        new[] {100, 200, 300},
        new[] {200, 100})]
    [InlineData(
        new[] { 100 },
        new[] {100})]
    [InlineData(
        new[] { 200, 400, 100},
        new [] {200, 100})]
    [InlineData(
        new[] {200, 200, 200},
        new [] {200, 200})]
    [InlineData(
        new[] {200, 300, 500, 400, 100},
        new[] {300, 200, 100})]
    public void SelectCandidates_ReturnValidResult(int[] bandwidths, int[] expectedResult)
    {   
        var hlsVariants = bandwidths.Select(x => new HlsVariantStub(x)).ToArray();
        var selector = new HlsSelector();

        var result = selector.SelectCandidates(hlsVariants);
        var selectedResult = result.Select(x => x.AverageBandwidth ?? x.Bandwidth).ToList();
        
        Assert.Equal(expectedResult, selectedResult);
    }

    [Theory]
    [MemberData(nameof(ValidCandidatesWithAverageBandwidthCases))]
    public void SelectCandidates_WhenDifferentAverageBandwidth_ReturnsValidResult((int Bandwidth, int? AverageBandwidth)[] bandWidthPair, int[] expectedResult)
    {
        var hlsVariants = bandWidthPair.Select(x => new HlsVariantStub(x.Bandwidth, x.AverageBandwidth)).ToArray();
        var selector = new HlsSelector();

        var result = selector.SelectCandidates(hlsVariants);
        var selectedResult = result.Select(x => x.AverageBandwidth ?? x.Bandwidth).ToList();
        
        Assert.Equal(expectedResult, selectedResult);
    }

    [Fact]
    public void SelectCandidates_WhenHasEmptyList_ReturnsEmptyList()
    {
        var selector = new HlsSelector();
        
        var result = selector.SelectCandidates([]);
        
        Assert.Empty(result);
    }

    public static TheoryData<(int Bandwidth, int? AverageBandwidth)[], int[]> ValidCandidatesWithAverageBandwidthCases => new()
    {
        {
            [
                (Bandwidth: 100, AverageBandwidth: null),
                (Bandwidth: 200, AverageBandwidth: 200),
                (Bandwidth: 300, AverageBandwidth: null),
                (Bandwidth: 400, AverageBandwidth: 400)
            ],
            [200, 100]
        },
        {
            [
                (Bandwidth: 100, AverageBandwidth: 100),
                (Bandwidth: 300, AverageBandwidth: 300),
                (Bandwidth: 400, AverageBandwidth: null)
            ],
            [300, 100]
        },
        {
            [(Bandwidth: 100, AverageBandwidth: 100)],
            [100]
        },
        {
            [
                (Bandwidth: 300, AverageBandwidth: 300),
                (Bandwidth: 100, AverageBandwidth: 100),
                (Bandwidth: 400, AverageBandwidth: 400),
                (Bandwidth: 200, AverageBandwidth: 200)
            ],
            [200, 100]
        },
        {
            [
                (Bandwidth: 200, AverageBandwidth: 200),
                (Bandwidth: 200, AverageBandwidth: 200),
                (Bandwidth: 200, AverageBandwidth: 200)
            ],
            [200, 200]
        },
        {
            [],
            []
        },
        {
            [
                (Bandwidth: 100, AverageBandwidth: 200),
                (Bandwidth: 200, AverageBandwidth: 300),
                (Bandwidth: 300, AverageBandwidth: 500),
                (Bandwidth: 400, AverageBandwidth: 400),
                (Bandwidth: 500, AverageBandwidth: 100)
            ],
            [300, 200, 100]
        }
    };
}