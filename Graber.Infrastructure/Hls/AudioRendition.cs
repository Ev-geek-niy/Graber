namespace Graber.Infrastructure.Hls;

public class AudioRendition
{
    public required string GroupId  { get; init; } 
    public required Uri Uri { get; init; }
    public string? Name { get; init; }
    public string? Language { get; init; }
    public bool IsDefault { get; init; }
    public bool AutoSelect { get; init; }
}