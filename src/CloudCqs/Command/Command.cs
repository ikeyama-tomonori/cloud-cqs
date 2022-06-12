namespace CloudCqs.Command;

public abstract class Command<TRequest> : Repository<TRequest, object>, ICommand<TRequest>
    where TRequest : notnull
{
    protected Command(CloudCqsOptions option) : base(option)
    {
    }
}
