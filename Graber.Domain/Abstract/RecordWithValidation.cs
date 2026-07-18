namespace Graber.Domain.Abstract;

public abstract record RecordWithValidation
{
    protected RecordWithValidation()
    {
        Validate();
    }

    protected RecordWithValidation(RecordWithValidation other)
    {
        Validate();
    }
    
    protected virtual void Validate()
    {
    }
}