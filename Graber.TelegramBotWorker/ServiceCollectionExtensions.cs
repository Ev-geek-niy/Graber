using Graber.TelegramBotWorker.Errors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;

namespace Graber.TelegramBotWorker;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddPresentation(IConfiguration configuration)
        {
            var botToken = configuration["Telegram:BotToken"]
                           ?? throw new InvalidOperationException(
                               "Telegram bot token is not configured. Set Telegram__BotToken.");;

            services
                .AddTelegramBot(botToken)
                .AddProviders();
        
            return services;
        }

        private IServiceCollection AddTelegramBot(string botToken)
        {
            services.AddSingleton<ITelegramBotClient>(
                new TelegramBotClient(botToken));
            services.AddHostedService<TelegramBotWorker>();
        
            return services;
        }

        private IServiceCollection AddProviders()
        {
            services.AddSingleton<ErrorMessageProvider>();
        
            return services;
        }
    }
}