using Graber.Application;
using Graber.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace Graber.TelegramBotWorker;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = Host.CreateApplicationBuilder(args);

        builder.Services
            .AddApplication()
            .AddInfrastructure();

        var botToken = builder.Configuration["Telegram:BotToken"]
            ?? throw new InvalidOperationException(
                "Telegram bot token is not configured. Set Telegram__BotToken.");

        builder.Services.AddSingleton<ITelegramBotClient>(
            new TelegramBotClient(botToken));
        builder.Services.AddHostedService<TelegramBotWorker>();

        await builder.Build().RunAsync();
    }
}
