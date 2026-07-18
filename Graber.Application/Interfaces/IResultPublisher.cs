using Graber.Application.Models;

namespace Graber.Application.Interfaces;

public interface IResultPublisher
{
    public Result<Video> Publish(Video video);
}