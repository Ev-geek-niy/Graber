namespace Graber.Application.Errors;

public sealed record MetadataError(
    MetadataErrorCode Code) : Error;