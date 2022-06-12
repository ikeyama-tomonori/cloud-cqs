namespace CloudCqs.Command;

public abstract class Command<TRequest> : Repository<TRequest, Void>, ICommand<TRequest>
    where TRequest : notnull
{
    protected Command(CloudCqsOptions option) : base(option)
    {
    }
}
