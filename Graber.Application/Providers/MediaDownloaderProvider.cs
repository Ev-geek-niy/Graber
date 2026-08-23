using Graber.Application.Interfaces;

namespace Graber.Application.Providers;

public class MediaDownloaderProvider(
    IEnumerable<IMediaDownloader> downloaders)
{
    public IMediaDownloader? GetDownloader(string url) =>
        downloaders.FirstOrDefault(downloader => downloader.CanExecute(url));
}