using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class StubScraper : IScraper
{
    private bool CanExecuteResult { get; }
    private string ResultValue { get; }
    private ScrapingError?  ErrorValue { get; }
    public StubScraper(bool canExecuteResult, string resultValue = "TestValue", ScrapingError? errorValue = null)
    {
        CanExecuteResult = canExecuteResult;
        ResultValue = resultValue;
        ErrorValue = errorValue;
    }
    
    public bool CanExecute(string input) => CanExecuteResult;

    public Task<Result<string>> ExecuteAsync(string input) =>
        Task.FromResult(new Result<string>(ErrorValue is not null,  ResultValue, ErrorValue));
}