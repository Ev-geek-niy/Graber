# Graber Architecture

## Назначение

Graber — Telegram-бот на .NET для получения пользовательской ссылки, скачивания медиа и отправки результата пользователю.

Документ описывает фактическую архитектуру текущей реализации. Планируемые изменения находятся в `ROADMAP.md`, а принятые архитектурные решения — в `DECISIONS.md`.

## Архитектурный стиль

Решение разделено на четыре слоя:

```text
Presentation → Application → Domain
       │
       └──────→ Infrastructure → Application → Domain
```

Фактические зависимости проектов:

- `Graber.Domain` не имеет project references;
- `Graber.Application` зависит от `Graber.Domain`;
- `Graber.Infrastructure` зависит от `Graber.Application`;
- `Graber.TelegramBotWorker` зависит от `Graber.Application` и `Graber.Infrastructure` и является composition root.

## Проекты и ответственности

### Graber.Domain

Содержит текущую доменную модель `VideoMetadata` и базовый тип `RecordWithValidation`.

Слой не зависит от Telegram, PuppeteerSharp, FFMpegCore или FFprobe.

### Graber.Application

Содержит:

- `ProcessUrlUseCase`;
- интерфейсы инфраструктурных обработчиков;
- `ScraperProvider` и `MediaDownloaderProvider`;
- `Result<T>` и модель ошибок;
- модель результата `Video`;
- регистрацию application-сервисов.

`ProcessUrlUseCase` выбирает scraper через `ScraperProvider` и возвращает `Result<Video>`.

### Graber.Infrastructure

Содержит реализации внешних операций:

- `XScraper` использует PuppeteerSharp;
- `FFMpegHlsDownloader` использует FFMpegCore;
- `MetadataExtractor` использует FFprobe;
- `ServiceCollectionExtensions` регистрирует реализации application-интерфейсов.

### Graber.TelegramBotWorker

Является Presentation-слоем и composition root.

Слой:

- создаёт Generic Host;
- регистрирует Application и Infrastructure;
- создаёт `ITelegramBotClient`;
- получает Telegram updates через long polling;
- передаёт текст сообщения в `ProcessUrlUseCase`;
- отправляет ошибку или результирующий файл пользователю;
- освобождает результирующий поток после отправки.

## Текущий runtime pipeline

```text
Telegram message
    ↓
TelegramBotWorker.HandleUpdateAsync
    ↓
ProcessUrlUseCase.ExecuteAsync
    ↓
ScraperProvider.GetScraper
    ↓
XScraper.ExecuteAsync
    ├── GetPlaylistUrlAsync
    │   ├── подготовка и запуск Chromium
    │   ├── открытие страницы X
    │   └── ожидание первого response с URL, содержащим ".m3u8"
    ├── FFMpegHlsDownloader.ExecuteAsync
    ├── MetadataExtractor.ExtractAsync
    └── создание Video
    ↓
TelegramBotWorker.SendDocument
```

В текущей реализации общая orchestration скачивания и извлечения metadata находится внутри `XScraper`. `ProcessUrlUseCase` выбирает scraper и делегирует ему выполнение полного сценария.

## Получение HLS

`XScraper.GetPlaylistUrlAsync`:

1. вызывает `BrowserFetcher.DownloadAsync`;
2. запускает Chromium с `Headless = false`;
3. начинает ожидать первый response, URL которого содержит `.m3u8`;
4. открывает страницу с ожиданием `DOMContentLoaded`;
5. ожидает HLS response не более 15 секунд;
6. возвращает URL либо application error.

Текущая реализация не анализирует содержимое playlist, не классифицирует его как master/media и не выбирает профиль качества явно.

## Скачивание медиа

`FFMpegHlsDownloader` принимает HLS URL и передаёт его FFMpegCore.

FFmpeg:

- копирует video и audio codecs без перекодирования;
- формирует MP4;
- использует fragmented MP4 flags;
- записывает результат в `MemoryStream`;
- возвращает поток с позицией `0`.

`FFMpegHlsDownloader.CanExecute` в текущей реализации возвращает `true` для любого входа.

## Извлечение metadata

`MetadataExtractor` получает уже существующий `Stream` и анализирует его через FFprobe.

Из результата формируется `VideoMetadata` с именем файла, форматом, MIME type, длительностью, шириной и высотой. Для seekable stream исходная позиция сохраняется и восстанавливается после анализа.

Текущая реализация обращается к `PrimaryVideoStream` и предполагает его наличие.

## Ошибки

Ожидаемый результат операций представлен через `Result<T>`.

Текущие типы ошибок:

- `PrivateVideo`;
- `DeleteVideo`;
- `NotFoundVideo`;
- `ServiceNotSupported`;
- `NetworkError`.

`ProcessUrlUseCase` возвращает `ServiceNotSupported`, если подходящий scraper не найден. Таймаут ожидания `.m3u8` в `XScraper` возвращается как `NotFoundVideo`.

## Dependency Injection

Scoped-сервисы:

- `ProcessUrlUseCase`;
- `ScraperProvider`;
- `MediaDownloaderProvider`;
- `IScraper` / `XScraper`;
- `IMediaDownloader` / `FFMpegHlsDownloader`;
- `IMetadataExtractor` / `MetadataExtractor`.

Singleton-сервис:

- `ITelegramBotClient`.

`TelegramBotWorker` создаёт отдельный asynchronous scope для обработки сообщения.

## Тестирование

Решение содержит два тестовых проекта:

- `Graber.UnitTests` проверяет application-компоненты без реальной инфраструктуры;
- `Graber.IntegrationTests` проверяет X scraper, HLS downloader и metadata extractor с реальными внешними ресурсами.

Текущие integration-тесты используют живые URL X и CDN, поэтому их результат зависит от сети, доступности внешнего контента, Chromium, FFmpeg и FFprobe.

## Известные ограничения текущей реализации

- orchestration полного media pipeline находится внутри `XScraper`;
- `MediaDownloaderProvider` зарегистрирован, но в runtime pipeline не используется;
- HLS playlist не классифицируется;
- политика выбора качества отсутствует;
- `CancellationToken` не проходит через application и infrastructure pipeline;
- Chromium запускается для каждого запроса;
- production headless mode ещё не настроен;
- результирующее видео полностью накапливается в памяти;
- максимальный размер результата не ограничен;
- `MetadataExtractor` предполагает наличие primary video stream;
- Telegram отправляет результат через `SendDocument`.
