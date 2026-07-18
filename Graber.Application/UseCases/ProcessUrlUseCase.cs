using Graber.Application.Enums;
using Graber.Application.Interfaces;
using Graber.Application.Models;
using Graber.Application.Providers;

namespace Graber.Application.UseCases;

public class ProcessUrlUseCase(
    ScraperProvider provider,
    IResultPublisher publisher
    )
{
    public Result<Video> Execute(string url)
    {
        throw new NotImplementedException();
    }
}