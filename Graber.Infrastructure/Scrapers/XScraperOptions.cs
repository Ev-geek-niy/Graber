namespace Graber.Infrastructure.Scrapers;

public sealed class XScraperOptions
{
    public const string SectionName = "XScraper";
    
    public TimeSpan PlaylistDiscoveryTimeout {get; set;} = TimeSpan.FromSeconds(15);
    public bool Headless { get; set; } = true;
}