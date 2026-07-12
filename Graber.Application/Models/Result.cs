namespace Graber.Application.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public T Value { get; init; }
    public ScrappingError Error { get; init; }
}