using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Domain.Models;

namespace Graber.UnitTests;

public class StubExtractor : IMetadataExtractor
{
    private VideoMetadata ResultValue { get; }
    private ScrapingError?  ErrorValue { get; }
    
    public StubExtractor(VideoMetadata? resultValue = null, ScrapingError? resultError = null)
    {
        ResultValue = resultValue ?? new VideoMetadata("FileName", "mp4", "video/mp4", TimeSpan.FromSeconds(10), 200, 200);
        ErrorValue = resultError;
    }
    
    public Task<Result<VideoMetadata>> ExtractAsync(Stream stream)
    {
        return Task.FromResult(new Result<VideoMetadata>(ErrorValue is not null, ResultValue, ErrorValue));
    }
}