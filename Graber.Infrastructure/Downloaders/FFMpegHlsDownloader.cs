using Graber.Application.Interfaces;
using Graber.Application.Models;
using FFMpegCore;
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

    public async Task<Result<Stream>> ExecuteAsync(string hlsPlaylistUrl)
    {
        var buffer = bufferFactory.Create();
        
        try
        {
            await DownloadAsync(hlsPlaylistUrl, buffer);
            return Result<Stream>.Success(buffer);
        }
        catch (Exception)
        {
            await buffer.DisposeAsync();
            return Result<Stream>.Failure(new DownloadError(DownloadErrorCode.DownloadFailed));
        }
    }

    private async Task DownloadAsync(string hlsPlaylistUrl, Stream buffer)
    {
        await FFMpegArguments.FromUrlInput(new Uri(hlsPlaylistUrl, UriKind.Absolute))
            .OutputToPipe(new StreamPipeSink(buffer), options => options
                .WithVideoCodec("copy")
                .WithAudioCodec("copy")
                .WithCustomArgument(
                    "-movflags +frag_keyframe+empty_moov+default_base_moof")
                .ForceFormat("mp4"))
            .ProcessAsynchronously();

        buffer.Position = 0;
    }
}