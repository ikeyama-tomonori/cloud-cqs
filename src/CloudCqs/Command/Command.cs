namespace CloudCqs.Command
{
    public abstract class Command<TRequest> : Repository<TRequest, Void>, ICommand<TRequest>
        where TRequest : notnull
    {
        protected class Handler : Handler<TRequest, Void>
        {
        }

        protected Command(CloudCqsOptions option) : base(option)
        {
        }
    }
}
