using Graber.Application.Errors;
using Graber.TelegramBotWorker.Errors;

namespace Graber.UnitTests;

public class ErrorMessageProviderTest
{
    [Theory]
    [InlineData(
        PipelineErrorCode.DownloadMethodNotSupported,
        "Для ресурса недоступен способ скачивания контента.")]
    [InlineData(
        PipelineErrorCode.SourceNotSupported,
        "Ресурс не поддерживается для обработки.")]
    [InlineData(
        (PipelineErrorCode)999,
        "Произошла неизвестная ошибка при выполнении скачивания.")]
    public void GetMessage_WhenPipelineErrorProvided_ReturnsExpectedMessage(PipelineErrorCode code, string expectedMessage)
    {
        var provider = new ErrorMessageProvider();
        var error = new PipelineError(code);

        var message = provider.GetMessage(error);
        
        Assert.Equal(expectedMessage, message);
    }
    
    [Theory]
    [InlineData(
        ScrapingErrorCode.MediaNotFound,
        "На странице не удалось обнаружить медиа-контент.")]
    [InlineData(
        ScrapingErrorCode.MediaDiscoveryFailed,
        "Не удалось получить ссылку на медиа-контент.")]
    [InlineData(
        ScrapingErrorCode.MediaRemoved,
        "Медиа-контент был удален.")]
    [InlineData(
        ScrapingErrorCode.MediaPrivate,
        "Медиа-контент ограничен правилами приватности.")]
    [InlineData(
        ScrapingErrorCode.SourceUnavailable,
        "Не удалось загрузить страницу.")]
    [InlineData(
        (ScrapingErrorCode)999,
        "Произошла неизвестная ошибка при сборе информации со страницы.")]
    public void GetMessage_WhenScrapingErrorProvided_ReturnsExpectedMessage(ScrapingErrorCode code, string expectedMessage)
    {
        var provider = new ErrorMessageProvider();
        var error = new ScrapingError(code);
        
        var message = provider.GetMessage(error);
        
        Assert.Equal(expectedMessage, message);
    }
    
    [Theory]
    [InlineData(
        DownloadErrorCode.DownloadFailed,
        "Не удалось скачать медиа-контент.")]
    [InlineData(
        (DownloadErrorCode)999,
        "Произошла ошибка при скачивании.")]
    public void GetMessage_WhenDownloadErrorProvided_ReturnsExpectedMessage(DownloadErrorCode code, string expectedMessage)
    {
        var provider = new ErrorMessageProvider();
        var error = new DownloadError(code);
        
        var message = provider.GetMessage(error);
        
        Assert.Equal(expectedMessage, message);
    }

    [Theory]
    [InlineData(
        MetadataErrorCode.ExtractionFailed,
        "Не удалось получить метаданные медиа-контента.")]
    [InlineData(
        MetadataErrorCode.UnsupportedFormat,
        "Неподдерживаемый формат файла.")]
    [InlineData(
        (MetadataErrorCode)999,
        "Произошла ошибка при получении метаданных медиа-контента.")]
    public void GetMessage_WhenMetadataErrorProvided_ReturnsExpectedMessage(MetadataErrorCode code, string expectedMessage)
    {
        var provider = new ErrorMessageProvider();
        var error = new MetadataError(code);
        
        var message = provider.GetMessage(error);
        
        Assert.Equal(expectedMessage, message);
    }

    [Fact]
    public void GetMessage_WhenErrorIsNull_ThrowsArgumentNullException()
    {
        var provider = new ErrorMessageProvider();
        Error error = null!;
        
        Assert.Throws<ArgumentNullException>(() => provider.GetMessage(error));
    }

    [Fact]
    public void GetMessage_WhenErrorTypeIsUnknown_ReturnsFallbackMessage()
    {
        var provider = new ErrorMessageProvider();
        var error = new UnknownError();
        
        var message = provider.GetMessage(error);
        
        Assert.Equal("Произошла неизвестная ошибка.", message);
    }

    [Fact]
    public void GetMessage_ForEveryPipelineErrorCode_DoesNotReturnFallback()
    {
        const string fallbackMessage = "Произошла неизвестная ошибка при выполнении скачивания.";
        var codes = Enum.GetValues<PipelineErrorCode>();
        var provider = new ErrorMessageProvider();
        
        Assert.All(codes, code =>
        {
            var error = new PipelineError(code);
            var message = provider.GetMessage(error);
            
            Assert.NotEqual(fallbackMessage, message);
        });
    }

    [Fact]
    public void GetMessage_ForEveryScrapingErrorCode_DoesNotReturnFallback()
    {
        const string fallbackMessage = "Произошла неизвестная ошибка при сборе информации со страницы.";
        var codes = Enum.GetValues<ScrapingErrorCode>();
        var provider = new ErrorMessageProvider();
        
        Assert.All(codes, code =>
        {
            var error = new ScrapingError(code);
            var message = provider.GetMessage(error);
            
            Assert.NotEqual(fallbackMessage, message);
        });
    }
    
    [Fact]
    public void GetMessage_ForEveryDownloadErrorCode_DoesNotReturnFallback()
    {
        const string fallbackMessage = "Произошла ошибка при скачивании.";
        var codes = Enum.GetValues<DownloadErrorCode>();
        var provider = new ErrorMessageProvider();

        Assert.All(codes, code =>
        {
            var error = new DownloadError(code);
            var message = provider.GetMessage(error);
            
            Assert.NotEqual(fallbackMessage, message);
        });
    }
    
    [Fact]
    public void GetMessage_ForEveryMetadataErrorCode_DoesNotReturnFallback()
    {
        const string fallbackMessage = "Произошла ошибка при получении метаданных медиа-контента.";
        var codes = Enum.GetValues<MetadataErrorCode>();
        var provider = new ErrorMessageProvider();

        Assert.All(codes, code =>
        {
            var error = new MetadataError(code);
            var message = provider.GetMessage(error);
            
            Assert.NotEqual(fallbackMessage, message);
        });
    }
    
    private sealed record UnknownError : Error;
}

