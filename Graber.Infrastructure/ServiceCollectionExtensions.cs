using Graber.Application.Interfaces;
using Graber.Infrastructure.Downloaders;
using Graber.Infrastructure.Extractors;
using Graber.Infrastructure.Factories;
using Graber.Infrastructure.Providers;
using Graber.Infrastructure.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Graber.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddInfrastructure(IConfiguration  configuration)
        {
            services
                .AddOptions<XScraperOptions>()
                .Bind(configuration.GetSection(XScraperOptions.SectionName))
                .Validate(
                    options => options.PlaylistDiscoveryTimeout > TimeSpan.Zero,
                    "XScraper playlist discovery timeout must be greater than zero.")
                .ValidateOnStart();
            
            services.AddScoped<IScraper, XScraper>();
            services.AddScoped<IMetadataExtractor, MetadataExtractor>();
            services.AddScoped<IMediaDownloader, FFMpegHlsDownloader>();
            services.AddSingleton<IMediaBufferFactory, MemoryStreamMediaBufferFactory>();
            services.AddSingleton<ChromiumBrowserProvider>();
            
            return services;
        }
    }
}
