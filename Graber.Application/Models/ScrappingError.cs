using Graber.Application.Enums;

namespace Graber.Application.Models;

public class ScrappingError(ScrappingErrorTypes type, string message)
{
    public ScrappingErrorTypes Type { get; init; } = type;
    public string Message { get; init; } = message;
}