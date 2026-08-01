using Graber.Application.Interfaces;
using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Extractors;
using Graber.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;

namespace Graber.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IScraper, XScraper>();
        serviceCollection.AddScoped<IMetadataExtractor, MetadataExtractor>();
        serviceCollection.AddScoped<IMediaDownloader, FFMpegHlsDownloader>();

        return serviceCollection;
    }
}
