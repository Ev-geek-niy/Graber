using Graber.Application.UseCases;
using Graber.TelegramBotWorker.Errors;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace Graber.TelegramBotWorker;

public class TelegramBotWorker(
    ITelegramBotClient botClient,
    IServiceScopeFactory scopeFactory,
    ErrorMessageProvider errorMessageProvider,
    ILogger<TelegramBotWorker> logger) : BackgroundService
{
    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiverOptions = new ReceiverOptions
        {
            AllowedUpdates = [UpdateType.Message]
        };

        return botClient.ReceiveAsync(
            HandleUpdateAsync,
            HandleErrorAsync,
            receiverOptions,
            stoppingToken);
    }

    private async Task HandleUpdateAsync(
        ITelegramBotClient client,
        Update update,
        CancellationToken cancellationToken)
    {
        var message = update.Message;
        if (message?.Text is not { } url)
            return;

        await using var scope = scopeFactory.CreateAsyncScope();
        var useCase = scope.ServiceProvider.GetRequiredService<ProcessUrlUseCase>();
        var result = await useCase.ExecuteAsync(url, cancellationToken);

        if (result.IsFailure)
        {
            await client.SendMessage(
                message.Chat.Id,
                errorMessageProvider.GetMessage(result.Error),
                cancellationToken: cancellationToken);
            return;
        }

        await using var videoStream = result.Value.VideoStream;
        await client.SendDocument(
            message.Chat.Id,
            InputFile.FromStream(videoStream, "video.mp4"),
            cancellationToken: cancellationToken);
    }

    private Task HandleErrorAsync(
        ITelegramBotClient client,
        Exception exception,
        CancellationToken cancellationToken)
    {
        logger.LogError(exception, "Telegram polling failed");
        return Task.CompletedTask;
    }
}
