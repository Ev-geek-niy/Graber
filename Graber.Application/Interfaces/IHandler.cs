using Graber.Application.Models;

namespace Graber.Application.Interfaces;

public interface IHandler<TInput, TOutput>
{
    public bool CanExecute(TInput input);
    public Task<Result<TOutput>> ExecuteAsync(TInput input);
}