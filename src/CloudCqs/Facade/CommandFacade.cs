namespace CloudCqs.Facade
{
    public abstract class CommandFacade<TRequest> : Facade<TRequest, object>, ICommandFacade<TRequest>
        where TRequest : notnull
    {
        protected CommandFacade(LogContext logContext) : base(logContext)
        {
        }
    }
}
