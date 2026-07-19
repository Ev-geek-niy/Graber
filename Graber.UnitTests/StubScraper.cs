using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class StubScraper(bool canExecuteResult) : IScraper
{
    public bool CanExecute(string input) => canExecuteResult;
    public Task<Result<Video>> ExecuteAsync(string input)
    {
        throw new NotImplementedException();
    }
}