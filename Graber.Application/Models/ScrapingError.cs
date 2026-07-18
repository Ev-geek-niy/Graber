using Graber.Application.Enums;

namespace Graber.Application.Models;

public record ScrapingError(ScrapingErrorType Type, string Message);