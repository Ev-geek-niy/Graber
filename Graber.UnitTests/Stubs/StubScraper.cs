using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.UnitTests.Stubs;

public class StubScraper : IScraper
{
    private bool CanExecuteResult { get; }
    private string? ResultValue { get; }
    private Error?  ErrorValue { get; }
    public StubScraper(bool canExecuteResult, string resultValue = "TestValue", ScrapingError? errorValue = null)
    {
        CanExecuteResult = canExecuteResult;
        ResultValue = resultValue;
        ErrorValue = errorValue;
    }
    
    public bool CanExecute(string input) => CanExecuteResult;

    public Task<Result<string>> ExecuteAsync(string input, CancellationToken ct)
    {
        ct.ThrowIfCancellationRequested();
        return Task.FromResult(ErrorValue is null && ResultValue is not null
            ? Result<string>.Success(ResultValue!)
            : Result<string>.Failure(ErrorValue!));
    }
        
}