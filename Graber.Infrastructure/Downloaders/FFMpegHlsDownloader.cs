using Graber.Application.Interfaces;
using Graber.Application.Models;
using FFMpegCore;
using FFMpegCore.Exceptions;
using FFMpegCore.Pipes;
using Graber.Application.Errors;
using Graber.Infrastructure.Factories;

namespace Graber.Infrastructure.Downloaders;

public class FFMpegHlsDownloader(IMediaBufferFactory bufferFactory) : IMediaDownloader
{
    public bool CanExecute(string input)
    {
        return true;
    }

    public async Task<Result<Stream>> ExecuteAsync(string hlsPlaylistUrl, CancellationToken ct)
    {
        var buffer = bufferFactory.Create();
        var ownershipTransferred = false;

        try
        {
            await DownloadAsync(hlsPlaylistUrl, buffer, ct);
            ownershipTransferred = true;
            return Result<Stream>.Success(buffer);
        }
        catch (FFMpegException)
        {
            return Result<Stream>.Failure(new DownloadError(DownloadErrorCode.DownloadFailed));
        }
        finally
        {
            if (!ownershipTransferred)
                await buffer.DisposeAsync();
        }
    }

    private async Task DownloadAsync(string hlsPlaylistUrl, Stream buffer, CancellationToken ct)
    {
        await FFMpegArguments.FromUrlInput(new Uri(hlsPlaylistUrl, UriKind.Absolute))
            .OutputToPipe(new StreamPipeSink(buffer), options => options
                .WithVideoCodec("copy")
                .WithAudioCodec("copy")
                .WithCustomArgument(
                    "-movflags +frag_keyframe+empty_moov+default_base_moof")
                .ForceFormat("mp4"))
            .CancellableThrough(ct)
            .ProcessAsynchronously();

        buffer.Position = 0;
    }
}