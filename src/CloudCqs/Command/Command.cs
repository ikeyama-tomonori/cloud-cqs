namespace CloudCqs.Command
{
    using Internal;

    public abstract class Command<TRequest> : Base<TRequest, object>, ICommand<TRequest>
        where TRequest : notnull
    {
        protected class Handler : Handler<TRequest, object>
        {
        }

        protected Command(CloudCqsOption option) : base(option)
        {
        }
    }
}
