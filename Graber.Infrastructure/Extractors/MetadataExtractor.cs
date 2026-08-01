using FFMpegCore;
using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Domain.Models;

namespace Graber.Infrastructure.Extractors;

public class MetadataExtractor : IMetadataExtractor
{
    public async Task<Result<VideoMetadata>> ExtractAsync(Stream stream)
    {
        var initialPosition = stream.CanSeek
            ? stream.Position
            : (long?)null;

        try
        {
            var metadataAnalysis = await FFProbe.AnalyseAsync(stream);
            var metadata = new VideoMetadata(
                FileName: "video.mp4",
                Extension: metadataAnalysis.Format.FormatName,
                MimeType: "video/mp4",
                Duration: metadataAnalysis.Duration,
                Width: metadataAnalysis.PrimaryVideoStream!.Width,
                Height: metadataAnalysis.PrimaryVideoStream.Height);

            return Result.Success(metadata);
        }
        catch (Exception ex)
        {
            return Result.Failure(ScrapingErrorType.ServiceNotSupported);
        }
        finally
        {
            if (initialPosition.HasValue)
                stream.Position = initialPosition.Value;
        }
    }
}