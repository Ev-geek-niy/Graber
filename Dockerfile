FROM mcr.microsoft.com/dotnet/sdk:10.0-alpine3.23 AS build
WORKDIR /src

COPY ["Graber.Domain/Graber.Domain.csproj", "Graber.Domain/"]
COPY ["Graber.Application/Graber.Application.csproj", "Graber.Application/"]
COPY ["Graber.Infrastructure/Graber.Infrastructure.csproj", "Graber.Infrastructure/"]
COPY ["Graber.TelegramBotWorker/Graber.TelegramBotWorker.csproj", "Graber.TelegramBotWorker/"]

RUN dotnet restore "Graber.TelegramBotWorker/Graber.TelegramBotWorker.csproj"
COPY . .

RUN dotnet publish "Graber.TelegramBotWorker/Graber.TelegramBotWorker.csproj" \
    --configuration Release \
    --output /app/publish \
    --no-restore \
    /p:UseAppHost=false


FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine3.23 AS runtime

WORKDIR /app

USER root
RUN apk add --no-cache \
    chromium \
    ffmpeg \
    tini

COPY --from=build --chown=app:app /app/publish ./

ENV DOTNET_ENVIRONMENT=Production \
    XScraper__Headless=true \
    XScraper__BrowserExecutablePath=/usr/bin/chromium
    
USER app

ENTRYPOINT ["/sbin/tini", "--", "dotnet", "Graber.TelegramBotWorker.dll"]