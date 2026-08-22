using Graber.Application.Interfaces;
using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Extractors;
using Graber.Infrastructure.Factories;
using Graber.Infrastructure.Scrapers;
using Microsoft.Extensions.DependencyInjection;

namespace Graber.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection serviceCollection)
    {
        public IServiceCollection AddInfrastructure()
        {
            serviceCollection.AddScoped<IScraper, XScraper>();
            serviceCollection.AddScoped<IMetadataExtractor, MetadataExtractor>();
            serviceCollection.AddScoped<IMediaDownloader, FFMpegHlsDownloader>();
            serviceCollection.AddSingleton<IMediaBufferFactory, MemoryStreamMediaBufferFactory>();
            
            return serviceCollection;
        }
    }
}
