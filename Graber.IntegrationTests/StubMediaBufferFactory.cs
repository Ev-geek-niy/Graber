using Graber.Infrastructure.Factories;

namespace Graber.IntegrationTests;

public sealed class StubMediaBufferFactory(Stream stream) : IMediaBufferFactory
{
    public Stream Create() => stream;
}