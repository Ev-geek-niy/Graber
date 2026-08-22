using Graber.Application.Errors;

namespace Graber.Application.Models;

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly Error? _error;

    public T Value => IsSuccess
        ? _value!
        : throw new InvalidOperationException();
    
    public Error Error => IsFailure 
        ? _error!
        : throw new InvalidOperationException();
    
    public bool IsSuccess => _error is null;
    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        ArgumentNullException.ThrowIfNull(value);
        _value = value;
    }

    private Result(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        _error = error;
    }

    public static Result<T> Success(T value) => new Result<T>(value);
    public static Result<T> Failure(Error error) => new Result<T>(error);
}

