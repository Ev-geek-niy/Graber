using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Application.Providers;

namespace Graber.Application.UseCases;

public class ProcessUrlUseCase(
    ScraperProvider scraperProvider,
    IMetadataExtractor extractor,
    MediaDownloaderProvider downloaderProvider
    )
{
    public async Task<Result<Video>> ExecuteAsync(string url, CancellationToken ct)
    {
        var scraper = scraperProvider.GetScraper(url);
        if (scraper == null)
            return Result<Video>.Failure(new PipelineError(PipelineErrorCode.SourceNotSupported));

        var hlsUrlResult = await scraper.ExecuteAsync(url, ct);
        if (hlsUrlResult.IsFailure)
            return Result<Video>.Failure(hlsUrlResult.Error);

        var downloader = downloaderProvider.GetDownloader(hlsUrlResult.Value);
        if (downloader == null)
            return Result<Video>.Failure(new PipelineError(PipelineErrorCode.DownloadMethodNotSupported));
        
        var mediaResult = await downloader.ExecuteAsync(hlsUrlResult.Value, ct);
        if (mediaResult.IsFailure)
            return Result<Video>.Failure(mediaResult.Error);

        var stream = mediaResult.Value;
        var ownershipTransfered = false;
        try
        {
            var metadataResult = await extractor.ExtractAsync(mediaResult.Value, ct);
            if (metadataResult.IsFailure)
                return Result<Video>.Failure(metadataResult.Error);
            
            ownershipTransfered = true;
            var video = new Video(stream, metadataResult.Value);
            return Result<Video>.Success(video);
        }
        finally
        {
            if (!ownershipTransfered)
                await stream.DisposeAsync();
        }
    }
}