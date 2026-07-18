using Graber.Application.Enums;

namespace Graber.Application.Models;

public class Result<T>
{
    public bool IsFailure { get; init; }
    public bool IsSuccess => !IsFailure;
    public T Value { get; }
    public ScrapingError? Error { get; init; }

    internal Result(bool isFailure, T value, ScrapingError? error)
    {
        IsFailure = isFailure;
        Value = value;
        Error = error;
    }
    
    public static implicit operator Result<T>(Failure failure) => new(true, default(T), failure.Error);
}

public static class Result
{
    public static Failure Failure(ScrapingErrorType type) => new(type);
    public static Failure Failure(ScrapingErrorType type, string message) => new(type, message);
    public static Result<T> Success<T>(T value) => new(false, value, null);
}

public class Failure(ScrapingErrorType type, string message)
{
    public ScrapingError Error { get; init; } = new(type, message);
    public Failure(ScrapingErrorType type) : this(type, "проверка"){}
}