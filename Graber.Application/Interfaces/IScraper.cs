using Graber.Application.Models;

namespace Graber.Application.Interfaces;

public interface IScraper<TInput, TOutput>
{
    public bool CanExecute(TInput input);
    public Result<TOutput> Execute(TInput input);
}