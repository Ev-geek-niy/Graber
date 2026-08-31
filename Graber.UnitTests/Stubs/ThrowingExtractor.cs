using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Domain.Models;

namespace Graber.UnitTests.Stubs;

public class ThrowingExtractor(Exception exception) : IMetadataExtractor
{
    public Task<Result<VideoMetadata>> ExtractAsync(Stream stream, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromException<Result<VideoMetadata>>(exception); 
    }
}