using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class StubHlsDownloader : IMediaDownloader
{
    private bool CanExecuteResult { get; }
    private Stream? ResultValue { get; }
    private Error?  ErrorValue { get; }

    public StubHlsDownloader(bool canExecuteResult, Stream? result = null,  Error? resultError = null)
    {
        CanExecuteResult = canExecuteResult;
        ResultValue = result;
        ErrorValue = resultError;
    }
    public bool CanExecute(string input) => CanExecuteResult;

    public Task<Result<Stream>> ExecuteAsync(string input) =>
        Task.FromResult(ErrorValue is null && ResultValue is not null
            ? Result<Stream>.Success(ResultValue!)
            : Result<Stream>.Failure(ErrorValue!));
}