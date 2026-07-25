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
        try
        {
            var metadataAnalysis = await FFProbe.AnalyseAsync(stream);
            var metadata = new VideoMetadata(
                FileName: "Filename",
                Extension: metadataAnalysis.Format.FormatName,
                MimeType: "mpeg",
                Duration: metadataAnalysis.Duration,
                Width: metadataAnalysis.PrimaryVideoStream!.Width,
                Height: metadataAnalysis.PrimaryVideoStream.Height);

            return Result.Success(metadata);
        }
        catch (Exception ex)
        {
            return Result.Failure(ScrapingErrorType.ServiceNotSupported);
        }
    }
}