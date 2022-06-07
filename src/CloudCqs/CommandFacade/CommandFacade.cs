namespace CloudCqs.CommandFacade;

public abstract class CommandFacade<TRequest> : Facade.Facade<TRequest, Void>, ICommandFacade<TRequest>
    where TRequest : notnull
{
    protected CommandFacade(CloudCqsOptions option) : base(option)
    {
    }
}
