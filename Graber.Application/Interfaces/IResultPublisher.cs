using Graber.Application.Models;

namespace Graber.Application.Interfaces;

public interface IResultPublisher
{
    public void Publish(Result<Video> result);
    public Task PublishAsync(Result<Video> result);
}