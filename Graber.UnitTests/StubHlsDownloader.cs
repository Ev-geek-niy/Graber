using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class StubHlsDownloader : IMediaDownloader
{
    private bool CanExecuteResult { get; }
    private Stream? ResultValue { get; }
    private ScrapingError?  ErrorValue { get; }

    public StubHlsDownloader(bool canExecuteResult, Stream? result = null,  ScrapingError? resultError = null)
    {
        CanExecuteResult = canExecuteResult;
        ResultValue = result;
        ErrorValue = resultError;
    }
    public bool CanExecute(string input) => CanExecuteResult;

    public Task<Result<Stream>> ExecuteAsync(string input) =>
        Task.FromResult(new Result<Stream>(ErrorValue is not null, ResultValue, ErrorValue));
}