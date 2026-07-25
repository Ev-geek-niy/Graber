using Graber.Application.Interfaces;
using Graber.Application.Models;
using FFMpegCore;
using FFMpegCore.Pipes;
using Graber.Application.Enums;

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
                    .ForceFormat("mpegts"))
                .ProcessAsynchronously();

            memoryStream.Position = 0;
            
            return Result.Success<Stream>(memoryStream);
        }
        catch (Exception e)
        {
            return Result.Failure(ScrapingErrorType.ServiceNotSupported);
        }
    }
}