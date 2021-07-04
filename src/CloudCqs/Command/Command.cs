namespace CloudCqs.Command
{
    using Internal;

    public abstract class Command<TRequest> : Base<TRequest, object>, ICommand<TRequest>
        where TRequest : notnull
    {
        protected class Execution : Execution<TRequest, object>
        {
        }

        protected Command(LogContext logContext) : base(logContext)
        {
        }
    }
}
