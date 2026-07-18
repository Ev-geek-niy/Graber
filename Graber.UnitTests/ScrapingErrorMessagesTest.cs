using Graber.Application.Enums;
using Graber.Application.Models;

namespace Graber.UnitTests;

public class ScrapingErrorMessagesTest
{
    [Fact]
    public void GetMessage_ReturnCorrectText_ForServiceNotSupported()
    {
        var message = ScrapingErrorMessages.GetMessage(ScrapingErrorType.ServiceNotSupported);
        Assert.Equal("The service is not supported.", message);
    }

    [Fact]
    public void GetMessage_ReturnCorrectText_ForNotImplementedScraper()
    {
        var message = ScrapingErrorMessages.GetMessage((ScrapingErrorType)0);
        Assert.Equal("Not typed error message.", message);
    }
}