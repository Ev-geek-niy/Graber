using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Domain.Models;

namespace Graber.UnitTests;

public class ThrowingExtractor(Exception exception) : IMetadataExtractor
{
    public Task<Result<VideoMetadata>> ExtractAsync(Stream stream) => Task.FromException<Result<VideoMetadata>>(exception);
}