using Graber.Application;
using Graber.Application.Errors;
using Graber.Application.Interfaces;
using Graber.Infrastructure;
using Graber.TelegramBotWorker.Errors;
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
            .AddInfrastructure()
            .AddPresentation(builder.Configuration);

        await builder.Build().RunAsync();
    }
}
