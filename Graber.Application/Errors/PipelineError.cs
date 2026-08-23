namespace Graber.Application.Errors;

public sealed record PipelineError(
    PipelineErrorCode Code) :  Error;