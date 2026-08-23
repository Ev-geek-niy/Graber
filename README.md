# Graber

Graber — Telegram-бот на .NET 10 для скачивания видео из X. Бот обнаруживает HLS playlist через Chromium, скачивает медиа с помощью FFmpeg, извлекает metadata через FFprobe и отправляет результат пользователю.

Сейчас поддерживается только `x.com`.

## Docker image

Готовый multi-platform image опубликован в Docker Hub:

- [`evgeekniy/graber:latest`](https://hub.docker.com/r/evgeekniy/graber);
- `evgeekniy/graber:0.1.0` — первая зафиксированная версия;
- поддерживаемые платформы: `linux/amd64` и `linux/arm64`.

### Запуск

Создайте локальный файл `.env`:

```dotenv
Telegram__BotToken=your-bot-token
```

Не добавляйте `.env` или реальный Telegram-токен в Git, Dockerfile либо Docker image.

Запустите контейнер:

```bash
docker run --detach \
  --name graber \
  --restart unless-stopped \
  --cap-add=SYS_ADMIN \
  --env-file .env \
  evgeekniy/graber:latest
```

`SYS_ADMIN` требуется Chromium для запуска sandbox внутри текущего container runtime. Это широкая Linux capability: предоставляйте её только доверенному image и не добавляйте Chromium-флаг `--no-sandbox` без отдельной оценки рисков.

Посмотреть логи:

```bash
docker logs --follow graber
```

Остановить и удалить контейнер:

```bash
docker stop graber
docker rm graber
```

### Конфигурация

.NET Configuration преобразует двойное подчёркивание в разделитель секций.

| Environment variable | Обязательность | Назначение |
| --- | --- | --- |
| `Telegram__BotToken` | обязательно | Токен Telegram-бота |
| `XScraper__PlaylistDiscoveryTimeout` | необязательно | Timeout ожидания HLS playlist, по умолчанию `00:00:15` |
| `XScraper__Headless` | необязательно | Headless mode Chromium, в image задано `true` |
| `XScraper__BrowserExecutablePath` | необязательно | Путь к Chromium, в image задано `/usr/bin/chromium` |

## Локальная сборка

Требуется Docker с запущенным Docker Engine.

```bash
docker build --tag graber:local .
```

Локальный image запускается так же, как опубликованный:

```bash
docker run --detach \
  --name graber-local \
  --cap-add=SYS_ADMIN \
  --env-file .env \
  graber:local
```

## Публикация Docker image

Авторизуйтесь в Docker Hub через browser/device flow:

```bash
docker login
```

Соберите и сразу опубликуйте обе архитектуры:

```bash
docker buildx build \
  --platform linux/amd64,linux/arm64 \
  --tag evgeekniy/graber:latest \
  --tag evgeekniy/graber:0.1.0 \
  --push .
```

При следующей публикации замените `0.1.0` новой версией. `latest` указывает на актуальный проверенный image, а уже опубликованный version tag повторно не используется.

Проверить manifest:

```bash
docker buildx imagetools inspect evgeekniy/graber:latest
```

## Разработка

Для запуска из исходников необходимы:

- .NET 10 SDK;
- FFmpeg и FFprobe;
- доступ к сети для автоматического скачивания Chromium при первом запуске;
- доступ к Telegram API, X и media CDN.

Стабильные тесты находятся в `Graber.UnitTests`. Integration-тесты используют Chromium, FFmpeg, FFprobe и живые внешние ресурсы, поэтому зависят от локального окружения и сети.

Архитектура и дальнейшие задачи описаны в документах:

- [ARCHITECTURE.md](ARCHITECTURE.md);
- [DECISIONS.md](DECISIONS.md);
- [ROADMAP.md](ROADMAP.md).

Исходный код: [gitlab.com/Ev-geek-niy/graber](https://gitlab.com/Ev-geek-niy/graber).
