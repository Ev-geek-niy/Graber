using Graber.Infrastructure;
using Graber.Infrastructure.Scrapers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace Graber.UnitTests;

public class InfrastructureOptionsTest
{
    [Fact]
    public void XScraperOptions_WhenConfigurationIsValid_BindingValues()
    {
        var values = new Dictionary<string, string?>()
        {
            ["XScraper:PlaylistDiscoveryTimeout"] = "00:00:25",
            ["XScraper:Headless"] = "false",
            ["XScraper:BrowserExecutablePath"] = null
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        var options = provider
            .GetRequiredService<IOptions<XScraperOptions>>()
            .Value;
        
        Assert.Equal(TimeSpan.FromSeconds(25), options.PlaylistDiscoveryTimeout);
        Assert.False(options.Headless);
    }
    
    [Fact]
    public void XScraperOptions_WhenTimeoutIsZero_ThrowsValidationException()
    {
        var values = new Dictionary<string, string?>()
        {
            ["XScraper:PlaylistDiscoveryTimeout"] = "00:00:00",
            ["XScraper:Headless"] = "false",
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();

        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        
        var options = provider.GetRequiredService<IOptions<XScraperOptions>>();
        var exception = Assert.Throws<OptionsValidationException>(() => options.Value);

        Assert.Contains(
            "timeout must be greater than zero",
            exception.Message,
            StringComparison.InvariantCulture);
    }
    
    [Fact]
    public void XScraperOptions_WhenExecutablePathIsEmpty_ThrowsValidationException()
    {
        var values = new Dictionary<string, string?>()
        {
            ["XScraper:Headless"] = "false",
            ["XScraper:PlaylistDiscoveryTimeout"] = "00:00:25",
            ["XScraper:BrowserExecutablePath"] = ""
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        
        using var provider = services.BuildServiceProvider();
        
        var options = provider.GetRequiredService<IOptions<XScraperOptions>>();
        var exception = Assert.Throws<OptionsValidationException>(() => options.Value);
        
        Assert.Contains(
            "XScraper browser executable not found.",
            exception.Message,
            StringComparison.InvariantCulture);
    }
    
    [Fact]
    public void XScraperOptions_WhenExecutablePathIsTargetToNonExistingFile_ThrowsValidationException()
    {
        var values = new Dictionary<string, string?>()
        {
            ["XScraper:Headless"] = "false",
            ["XScraper:PlaylistDiscoveryTimeout"] = "00:00:25",
            ["XScraper:BrowserExecutablePath"] = "t/e/s/t"
        };
        
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
        
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);
        
        using var provider = services.BuildServiceProvider();
        
        var options = provider.GetRequiredService<IOptions<XScraperOptions>>();
        var exception = Assert.Throws<OptionsValidationException>(() => options.Value);
        
        Assert.Contains(
            "XScraper browser executable not found.",
            exception.Message,
            StringComparison.InvariantCulture);
    }
}