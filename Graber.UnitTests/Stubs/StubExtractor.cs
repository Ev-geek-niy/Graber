using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Domain.Models;

namespace Graber.UnitTests.Stubs;

public class StubExtractor : IMetadataExtractor
{
    private VideoMetadata? ResultValue { get; }
    private Error?  ErrorValue { get; }
    
    public StubExtractor(VideoMetadata? resultValue = null, Error? resultError = null)
    {
        ResultValue = resultValue ?? new VideoMetadata("FileName", "mp4", "video/mp4", TimeSpan.FromSeconds(10), 200, 200);
        ErrorValue = resultError;
    }
    
    public Task<Result<VideoMetadata>> ExtractAsync(Stream stream,  CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(ErrorValue is null && ResultValue is not null
            ? Result<VideoMetadata>.Success(ResultValue!)
            : Result<VideoMetadata>.Failure(ErrorValue!));
    }
}