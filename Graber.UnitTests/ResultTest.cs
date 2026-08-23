using Graber.Application.Errors;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class ResultTest
{
    [Fact]
    public void Success_WithValue_CreateSuccessfulResult()
    {
        var expectedValue = "Hello World";
        
        var result = Result<string>.Success(expectedValue);
        
        Assert.True(result.IsSuccess);
        Assert.False(result.IsFailure);
        Assert.Same(expectedValue, result.Value);
    }

    [Fact]
    public void Failure_WithError_CreateFailedResult()
    {
        var expectedError = new DownloadError(DownloadErrorCode.DownloadFailed);
        
        var result = Result<string>.Failure(expectedError);
        
        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Same(expectedError, result.Error);
    }
    
    [Fact]
    public void Success_WhenErrorAccessed_ThrowsInvalidOperationException()
    {
        var expectedValue = "Hello World";

        var result = Result<string>.Success(expectedValue);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = result.Error;
        });
    }
    
    [Fact]
    public void Failure_WhenValueAccessed_ThrowsInvalidOperationException()
    {
        var expectedError = new DownloadError(DownloadErrorCode.DownloadFailed);
        
        var result = Result<string>.Failure(expectedError);

        Assert.Throws<InvalidOperationException>(() =>
        {
            _ = result.Value;
        });
    }

    [Fact]
    public void Success_WhenValueIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Success(null!));
    }

    [Fact]
    public void Failure_WhenErrorIsNull_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => Result<string>.Failure(null!));
    }
}