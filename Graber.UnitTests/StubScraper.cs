using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class StubScraper(bool canExecuteResult) : IScraper<string, Video>
{
    public bool CanExecute(string input) => canExecuteResult;

    public Result<Video> Execute(string input)
    {
        throw new NotImplementedException();
    }
}