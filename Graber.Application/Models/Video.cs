using Graber.Domain.Models;

namespace Graber.Application.Models;

public class Video
{
    public Stream VideoStream { get; init; }
    public VideoMetadata Metadata { get; init; }
}