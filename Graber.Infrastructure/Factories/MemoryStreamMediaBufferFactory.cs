namespace Graber.Infrastructure.Factories;

public sealed class MemoryStreamMediaBufferFactory : IMediaBufferFactory
{
    public Stream Create() => new MemoryStream();
}