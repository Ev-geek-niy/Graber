using Graber.Application.Errors;

namespace Graber.TelegramBotWorker.Errors;

public sealed class ErrorMessageProvider
{
    public string GetMessage(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        return error switch
        {
            PipelineError pipeline => GetMessage(pipeline),
            ScrapingError scraping => GetMessage(scraping),
            DownloadError download => GetMessage(download),
            MetadataError metadata => GetMessage(metadata),
            _ => "Произошла неизвестная ошибка."
        };
    }

    private string GetMessage(PipelineError pipeline)
    {
        return pipeline.Code switch
        {
            PipelineErrorCode.DownloadMethodNotSupported => "Для ресурса недоступен способ скачивания контента.",
            PipelineErrorCode.SourceNotSupported => "Ресурс не поддерживается для обработки.",
            _ => "Произошла неизвестная ошибка при выполнении скачивания."
        };
    }

    private string GetMessage(ScrapingError scraping)
    {
        return scraping.Code switch
        {
            ScrapingErrorCode.MediaNotFound => "На странице не удалось обнаружить медиа-контент.",
            ScrapingErrorCode.MediaDiscoveryFailed => "Не удалось получить ссылку на медиа-контент.",
            ScrapingErrorCode.MediaPrivate => "Медиа-контент ограничен правилами приватности.",
            ScrapingErrorCode.MediaRemoved => "Медиа-контент был удален.",
            ScrapingErrorCode.SourceUnavailable => "Не удалось загрузить страницу.",
            _ => "Произошла неизвестная ошибка при сборе информации со страницы."
        };
    }

    private string GetMessage(DownloadError download)
    {
        return download.Code switch
        {
            DownloadErrorCode.DownloadFailed => "Не удалось скачать медиа-контент.",
            _ => "Произошла ошибка при скачивании."
        };
    }

    private string GetMessage(MetadataError metadata)
    {
        return metadata.Code switch
        {
            MetadataErrorCode.ExtractionFailed => "Не удалось получить метаданные медиа-контента.",
            MetadataErrorCode.UnsupportedFormat => "Неподдерживаемый формат файла.",
            _ => "Произошла ошибка при получении метаданных медиа-контента."
        };
    }
}