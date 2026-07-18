using Graber.Domain.Models;

namespace Graber.Application.Models;

public class Video(Stream videoStream, VideoMetadata metadata)
{
    public Stream VideoStream { get; init; } = videoStream;
    public VideoMetadata Metadata { get; init; } = metadata;
}