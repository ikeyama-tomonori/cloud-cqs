namespace CloudCqs.CommandFacade
{
    using Facade;

    public abstract class CommandFacade<TRequest> : Facade<TRequest, object>, ICommandFacade<TRequest>
        where TRequest : notnull
    {
        protected CommandFacade(CloudCqsOptions option) : base(option)
        {
        }
    }
}
