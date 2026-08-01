using Graber.Application.Providers;
using Graber.Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Graber.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<ScraperProvider>();
        serviceCollection.AddScoped<MediaDownloaderProvider>();
        serviceCollection.AddScoped<ProcessUrlUseCase>();

        return serviceCollection;
    }
}
