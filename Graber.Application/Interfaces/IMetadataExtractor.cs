using Graber.Application.Models;
using Graber.Domain.Models;

namespace Graber.Application.Interfaces;

public interface IMetadataExtractor
{
    public Task<Result<VideoMetadata>> ExtractAsync(Stream stream);
}