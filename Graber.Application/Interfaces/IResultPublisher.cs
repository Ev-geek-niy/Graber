using Graber.Application.Models;

namespace Graber.Application.Interfaces;

public interface IResultPublisher
{
    public Task PublishAsync(Result<Video> result);
}