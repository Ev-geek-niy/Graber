using Graber.Application.Interfaces;
using Graber.Application.Models;
using FFMpegCore;
using FFMpegCore.Pipes;
using Graber.Application.Errors;

namespace Graber.Infrastructure.Downloaders;

public class FFMpegHlsDownloader : IMediaDownloader
{
    public bool CanExecute(string input)
    {
        return true;
    }

    public async Task<Result<Stream>> ExecuteAsync(string hlsPlaylistUrl)
    {
        try
        {
            var memoryStream = new MemoryStream(); 
        
            await FFMpegArguments.FromUrlInput(new Uri(hlsPlaylistUrl))
                .OutputToPipe(new StreamPipeSink(memoryStream), options => options
                    .WithVideoCodec("copy")
                    .WithAudioCodec("copy")
                    .WithCustomArgument(
                        "-movflags +frag_keyframe+empty_moov+default_base_moof")
                    .ForceFormat("mp4"))
                .ProcessAsynchronously();

            memoryStream.Position = 0;
            
            return Result<Stream>.Success(memoryStream);
        }
        catch (Exception)
        {
            return Result<Stream>.Failure(new DownloadError(DownloadErrorCode.DownloadFailed));
        }
    }
}